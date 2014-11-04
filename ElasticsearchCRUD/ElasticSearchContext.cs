using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.ContextAlias;
using ElasticsearchCRUD.ContextClearCache;
using ElasticsearchCRUD.ContextCount;
using ElasticsearchCRUD.ContextDeleteByQuery;
using ElasticsearchCRUD.ContextGet;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Context for crud operations. 
	/// </summary>
	public class ElasticsearchContext : IDisposable 
	{	
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly HttpClient _client = new HttpClient();
		private readonly List<EntityContextInfo> _entityPendingChanges = new List<EntityContextInfo>();	
		private readonly string _connectionString;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;

		/// <summary>
		/// TraceProvider for all logs, trace etc. This can be replaced with any TraceProvider implementation.
		/// </summary>
		public ITraceProvider TraceProvider = new NullTraceProvider();

		/// <summary>
		/// This bool needs to be set to true if you want to delete an index. Per default this is false.
		/// </summary>
		public bool AllowDeleteForIndex { get; set; }

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="connectionString">Connection string which is used as ther base URL for the HttpClient</param>
		/// <param name="elasticsearchSerializerConfiguration">Configuration class for the context. The default settings can be oset here. This 
		/// class contains an IElasticsearchMappingResolver</param>
		public ElasticsearchContext(string connectionString, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticsearchContext with connection string: {0}", connectionString, "ElasticsearchContext");
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="connectionString">Connection string which is used as ther base URL for the HttpClient</param>
		/// <param name="elasticsearchMappingResolver">Resolver used for getting the index and type of a document type. The default 
		/// ElasticsearchSerializerConfiguration is used in this ctor.</param>
		public ElasticsearchContext(string connectionString, IElasticsearchMappingResolver elasticsearchMappingResolver)
		{
			_elasticsearchSerializerConfiguration = new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver);
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticsearchContext with connection string: {0}", connectionString, "ElasticsearchContext");
		}

		/// <summary>
		/// Adds a document to the pending changes list. Nor HTTP request is sent with this method. If the save changes is not called, the
		/// document is not added or updated in the search engine
		/// </summary>
		/// <param name="document">Document to be added or updated</param>
		/// <param name="id">document id</param>
		/// <param name="parentId">parent id of the document. This is only used if the ElasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex
		/// property is set to true. The document is then saved with the parent routing. It will also be saved even if the parent does not exist.</param>
		public void AddUpdateDocument(object document, object id, object parentId = null)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Adding document: {0}, {1} to pending list", document.GetType().Name, id, "ElasticsearchContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), EntityType = document.GetType(), Document = document, ParentId = parentId });
		}

		/// <summary>
		/// Adds a document to the pending changes list to be deletedd. Nor HTTP request is sent with this method. If the save changes is not called, the
		/// document is not added or updated in the search engine
		/// </summary>
		/// <typeparam name="T">This type is used to get the index and type of the document</typeparam>
		/// <param name="id">id of the document which will be deleted.</param>
		public void DeleteDocument<T>(object id)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Request to delete document with id: {0}, Type: {1}, adding to pending list", id, typeof(T).Name, "ElasticsearchContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), DeleteDocument = true, EntityType = typeof(T), Document = null});
		}

		/// <summary>
		/// Saves all the changes in the pending list of changes, add, update and delete. It also creates mappings and indexes
		/// if the child documents are saved as separate index types. ElasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex= true
		/// </summary>
		/// <returns>Returns HTTP response information</returns>
		public ResultDetails<string> SaveChangesAndInitMappingsForChildDocuments()
		{
			var elasticsearchContextAddDeleteUpdate = new ElasticsearchContextAddDeleteUpdate(
					TraceProvider,
					_cancellationTokenSource,
					_elasticsearchSerializerConfiguration,
					_client,
					_connectionString
			);

			return elasticsearchContextAddDeleteUpdate.SaveChanges(_entityPendingChanges, true);
		}

		/// <summary>
		/// Saves all the changes in the pending list of changes, add, update and delete. No mappings are created here for child document types.
		/// </summary>
		/// <returns>Returns HTTP response information</returns>
		public ResultDetails<string> SaveChanges()
		{
			var elasticsearchContextAddDeleteUpdate = new ElasticsearchContextAddDeleteUpdate(
					TraceProvider,
					_cancellationTokenSource,
					_elasticsearchSerializerConfiguration,
					_client,
					_connectionString
			);

			return elasticsearchContextAddDeleteUpdate.SaveChanges(_entityPendingChanges, false);
		}

		/// <summary>
		/// async Saves all the changes in the pending list of changes, add, update and delete. No mappings are created here for child document types.
		/// </summary>
		/// <returns>Returns HTTP response information</returns>
		public async Task<ResultDetails<string>> SaveChangesAsync()
		{
			var elasticsearchContextAddDeleteUpdate = new ElasticsearchContextAddDeleteUpdate(
					TraceProvider,
					_cancellationTokenSource,
					_elasticsearchSerializerConfiguration,
					_client,
					_connectionString
					);

			return await elasticsearchContextAddDeleteUpdate.SaveChangesAsync(_entityPendingChanges);
		}

		/// <summary>
		/// Gets a document by id. Elasticsearch GET API
		/// </summary>
		/// <typeparam name="T">type used for the document index and type definition</typeparam>
		/// <param name="documentId">document id</param>
		/// <param name="parentId">Parent Id of the document if document is a child document Id the Id is incorrect, you may still recieve the child document (parentId might be
		/// saved to the same shard.) If the child is a child document and no parent id is set, no docuemnt will be found.</param>
		/// <returns>Document type T</returns>
		public T GetDocument<T>(object documentId, object parentId = null)
		{
			var elasticsearchContextGet = new ElasticsearchContextGet(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextGet.GetDocument<T>(documentId, parentId);
		}

		/// <summary>
		/// Uses Elasticsearch search API to get the document per id
		/// </summary>
		/// <typeparam name="T">type T used to get index anf the type of the document.</typeparam>
		/// <param name="documentId"></param>
		/// <returns>Returns the document of type T</returns>
		public T SearchById<T>(object documentId)
		{
			var elasticsearchContextSearch = new ElasticsearchContextSearch(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextSearch.SearchById<T>(documentId);
		}

		/// <summary>
		/// Search API method to search for anything. any json string which matches the Elasticsearch Search API can be used. Only single index and type search
		/// </summary>
		/// <typeparam name="T">Type T used for the index and tpye used in the search</typeparam>
		/// <param name="searchJsonParameters">JSON string which matches the Elasticsearch Search API</param>
		/// <returns>A collection of documents of type T</returns>
		public ResultDetails<Collection<T>> Search<T>(string searchJsonParameters)
		{
			var search = new Search(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return search.PostSearch<T>(searchJsonParameters);
		}

		/// <summary>
		/// async Search API method to search for anything. any json string which matches the Elasticsearch Search API can be used. Only single index and type search
		/// </summary>
		/// <typeparam name="T">Type T used for the index and tpye used in the search</typeparam>
		/// <param name="searchJsonParameters">JSON string which matches the Elasticsearch Search API</param>
		/// <returns>A collection of documents of type T in a Task</returns>
		public async Task<ResultDetails<Collection<T>>> SearchAsync<T>(string searchJsonParameters)
		{
			var search = new Search(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await search.PostSearchAsync<T>(searchJsonParameters);
		}

		/// <summary>
		/// ElasticsearchContextCount to amount of hits for a index, type and query.
		/// </summary>
		/// <typeparam name="T">Type to find</typeparam>
		/// <param name="jsonContent">json query data, returns all in emtpy</param>
		/// <returns>Result amount of document found</returns>
		public long Count<T>(string jsonContent = "")
		{
			var count = new ElasticsearchContextCount(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return count.PostCount<T>(jsonContent).PayloadResult;
		}

		/// <summary>
		/// ElasticsearchContextCount to amount of hits for a index, type and query.
		/// </summary>
		/// <typeparam name="T">Type to find</typeparam>
		/// <param name="jsonContent">json query data, returns all in emtpy</param>
		/// <returns>Result amount of document found in a result details task</returns>
		public async Task<ResultDetails<long>> CountAsync<T>(string jsonContent = "")
		{
			var count = new ElasticsearchContextCount(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await count.PostCountAsync<T>(jsonContent);
		}

		/// <summary>
		/// async Uses Elasticsearch search API to get the document per id
		/// </summary>
		/// <typeparam name="T">type T used to get index anf the type of the document.</typeparam>
		/// <param name="documentId"></param>
		/// <returns>Returns the document of type T in a Task with result details</returns>
		public async Task<ResultDetails<T>> SearchByIdAsync<T>(object documentId)
		{
			var elasticsearchContextSearch = new ElasticsearchContextSearch(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextSearch.SearchByIdAsync<T>(documentId);
		}

		/// <summary>
		/// Async Deletes all documents found using the query in the body.
		/// </summary>
		/// <typeparam name="T">Type used to define the index and the type in Elasticsearch</typeparam>
		/// <param name="jsonContent">json string using directly in Elasticsearch API. </param>
		/// <returns>Returns true if ok</returns>
		public async Task<ResultDetails<bool>> DeleteByQueryAsync<T>(string jsonContent)
		{
			if (string.IsNullOrEmpty(jsonContent))
			{
				throw new ElasticsearchCrudException("Context: you must supply a json query for DeleteByQueryAsync");
			}

			var deleteByQueryApi = new ElasticsearchContextDeleteByQuery(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await deleteByQueryApi.DeleteByQueryAsync<T>(jsonContent);
		}

		/// <summary>
		/// Deletes all documents found using the query in the body.
		/// </summary>
		/// <typeparam name="T">Type used to define the index and the type in Elasticsearch</typeparam>
		/// <param name="jsonContent">json string using directly in Elasticsearch API. </param>
		/// <returns>Returns true if ok</returns>
		public ResultDetails<bool> DeleteByQuery<T>(string jsonContent)
		{
			if (string.IsNullOrEmpty(jsonContent))
			{
				throw new ElasticsearchCrudException("Context: you must supply a json query for DeleteByQueryAsync");
			}

			var deleteByQueryApi = new ElasticsearchContextDeleteByQuery(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return deleteByQueryApi.SendDeleteByQuery<T>(jsonContent);
		}

		/// <summary>
		/// Gets a document by id. Elasticsearch GET API
		/// </summary>
		/// <typeparam name="T">type used for the document index and type definition</typeparam>
		/// <param name="documentId">document id</param>
		/// <param name="parentId">Parent Id of the document if document is a child document Id the Id is incorrect, you may still recieve the child document (parentId might be
		/// saved to the same shard.) If the child is a child document and no parent id is set, no docuemnt will be found.</param>
		/// <returns>Document type T in a Task with result details</returns>
		public async Task<ResultDetails<T>> GetDocumentAsync<T>(object documentId, object parentId = null)
		{
			var elasticsearchContextGet = new ElasticsearchContextGet(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextGet.GetDocumentAsync<T>(documentId, parentId);

		}

		/// <summary>
		/// Checks if a document exists with a head request
		/// </summary>
		/// <typeparam name="T">Type of document to find</typeparam>
		/// <param name="documentId">Id of the document</param>
		/// <param name="parentId">parent Id, required if hte docuemtnis a child document and routing is required.
		/// NOTE: if the parent Id is incorrect but save on the same shard as the correct parentId, the document will be found!</param>
		/// <returns>true or false</returns>
		public bool DocumentExists<T>(object documentId, object parentId = null)
		{
			var elasticsearchContextGet = new ElasticsearchContextGet(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextGet.DocumentExists<T>(documentId, parentId);
		}

		/// <summary>
		/// Async Checks if a document exists with a head request
		/// </summary>
		/// <typeparam name="T">Type of document to find</typeparam>
		/// <param name="documentId">Id of the document</param>
		/// <param name="parentId">parent Id, required if hte docuemtnis a child document and routing is required.
		/// NOTE: if the parent Id is incorrect but save on the same shard as the correct parentId, the document will be found!</param>
		/// <returns>true or false</returns>
		public async Task<ResultDetails<bool>> DocumentExistsAsync<T>(object documentId, object parentId = null)
		{
			var elasticsearchContextGet = new ElasticsearchContextGet(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextGet.DocumentExistsAsync<T>(documentId, parentId);
		}

		/// <summary>
		/// Clears the cache for the index. The index is defined using the Type
		/// </summary>
		/// <typeparam name="T">Type used to get the index name</typeparam>
		/// <returns>returns true if cache has been cleared</returns>
		public bool ClearCacheForIndex<T>()
		{
			var elasticsearchContextClearCache = new ElasticsearchContextClearCache(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextClearCache.ClearCacheForIndex<T>();
		}

		/// <summary>
		/// Async Clears the cache for the index. The index is defined using the Type
		/// </summary>
		/// <typeparam name="T">Type used to get the index name</typeparam>
		/// <returns>returns true if cache has been cleared</returns>
		public async Task<ResultDetails<bool>> ClearCacheForIndexAsync<T>()
		{
			var elasticsearchContextClearCache = new ElasticsearchContextClearCache(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextClearCache.ClearCacheForIndexAsync<T>();
		}

		/// <summary>
		/// Creates a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias">name of the alias</param>
		/// <param name="index">index for the alias</param>
		/// <returns>true if the alias was created </returns>
		public bool AliasCreateForIndex(string alias, string index)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextAlias.SendAliasCommand(elasticsearchContextAlias.BuildCreateOrRemoveAlias(AliasAction.add,alias, index));
		}

		/// <summary>
		/// Async Creates a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias">name of the alias</param>
		/// <param name="index">index for the alias</param>
		/// <returns>true if the alias was created </returns>
		public async Task<ResultDetails<bool>> AliasCreateForIndexAsync(string alias, string index)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextAlias.SendAliasCommandAsync(elasticsearchContextAlias.BuildCreateOrRemoveAlias(AliasAction.add,alias, index));
		}

		/// <summary>
		/// Creates any alias command depending on the json content
		/// </summary>
		/// <param name="jsonContent">content for the _aliases, see Elasticsearch documnetation</param>
		/// <returns>returns true if the alias commnad was completed successfully</returns>
		public bool Alias(string jsonContent)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextAlias.SendAliasCommand(jsonContent);
		}

		/// <summary>
		/// Async Creates any alias command depending on the json content
		/// </summary>
		/// <param name="jsonContent">content for the _aliases, see Elasticsearch documnetation</param>
		/// <returns>returns true if the alias commnad was completed successfully</returns>
		public async Task<ResultDetails<bool>> AliasAsync(string jsonContent)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextAlias.SendAliasCommandAsync(jsonContent);
		}

		/// <summary>
		/// Removes a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias">name of the alias</param>
		/// <param name="index">index for the alias</param>
		/// <returns>true if the alias was removed </returns>
		public bool AliasRemoveForIndex(string alias, string index)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextAlias.SendAliasCommand(elasticsearchContextAlias.BuildCreateOrRemoveAlias(AliasAction.remove, alias, index));
		}

		/// <summary>
		/// asnyc Removes a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias">name of the alias</param>
		/// <param name="index">index for the alias</param>
		/// <returns>true if the alias was removed </returns>
		public async Task<ResultDetails<bool>> AliasRemoveForIndexAsync(string alias, string index)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextAlias.SendAliasCommandAsync(elasticsearchContextAlias.BuildCreateOrRemoveAlias(AliasAction.remove, alias, index));
		}

		/// <summary>
		/// Replaces the index for the alias. This can be used when reindexing live
		/// </summary>
		/// <param name="alias">Name of the alias</param>
		/// <param name="indexOld">Old index which will be removed</param>
		/// <param name="indexNew">New index which will be mapped to the alias</param>
		/// <returns>Returns true if the index was replaced</returns>
		public bool AliasReplaceIndex(string alias, string indexOld, string indexNew)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextAlias.SendAliasCommand(elasticsearchContextAlias.BuildAliasChangeIndex(alias, indexOld, indexNew));
		}

		/// <summary>
		/// Async Replaces the index for the alias. This can be used when reindexing live
		/// </summary>
		/// <param name="alias">Name of the alias</param>
		/// <param name="indexOld">Old index which will be removed</param>
		/// <param name="indexNew">New index which will be mapped to the alias</param>
		/// <returns>Returns true if the index was replaced</returns>
		public async Task<ResultDetails<bool>> AliasReplaceIndexAsync(string alias, string indexOld, string indexNew)
		{
			var elasticsearchContextAlias = new ElasticsearchContextAlias(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextAlias.SendAliasCommandAsync(elasticsearchContextAlias.BuildAliasChangeIndex(alias, indexOld, indexNew));
		}

		/// <summary>
		/// Delete the whole index if it exists and Elasticsearch allows delete index.
		/// Property AllowDeleteForIndex must also be set to true.
		/// </summary>
		/// <typeparam name="T">Type used to get the index to delete.</typeparam>
		/// <returns>Result details in a task</returns>
		public async Task<ResultDetails<T>> DeleteIndexAsync<T>()
		{
			var elasticsearchContextAddDeleteUpdate = new ElasticsearchContextAddDeleteUpdate(
					TraceProvider,
					_cancellationTokenSource,
					_elasticsearchSerializerConfiguration,
					_client,
					_connectionString
			);

			return await elasticsearchContextAddDeleteUpdate.DeleteIndexAsync<T>(AllowDeleteForIndex);
		}

		/// <summary>
		/// Dispose used to clean the HttpClient
		/// </summary>
		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
			}
		}
	}
}
