using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.ContextGet;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.CountApi;
using ElasticsearchCRUD.SearchApi;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Context for crud operations. 
	/// </summary>
	public class ElasticSearchContext : IDisposable 
	{	
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly HttpClient _client = new HttpClient();
		private readonly List<EntityContextInfo> _entityPendingChanges = new List<EntityContextInfo>();	
		private readonly string _connectionString;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
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
		/// class contains an IElasticSearchMappingResolver</param>
		public ElasticSearchContext(string connectionString, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticSearchContext with connection string: {0}", connectionString, "ElasticSearchContext");
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="connectionString">Connection string which is used as ther base URL for the HttpClient</param>
		/// <param name="elasticSearchMappingResolver">Resolver used for getting the index and type of a document type. The default 
		/// ElasticsearchSerializerConfiguration is used in this ctor.</param>
		public ElasticSearchContext(string connectionString, IElasticSearchMappingResolver elasticSearchMappingResolver)
		{
			_elasticsearchSerializerConfiguration = new ElasticsearchSerializerConfiguration(elasticSearchMappingResolver);
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticSearchContext with connection string: {0}", connectionString, "ElasticSearchContext");
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
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Adding document: {0}, {1} to pending list", document.GetType().Name, id, "ElasticSearchContext");
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
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Request to delete document with id: {0}, Type: {1}, adding to pending list", id, typeof(T).Name, "ElasticSearchContext");
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
		/// Count to amount of hits for a index, type and query.
		/// </summary>
		/// <typeparam name="T">Type to find</typeparam>
		/// <param name="jsonContent">json query data, returns all in emtpy</param>
		/// <returns>Result amount of document found</returns>
		public long Count<T>(string jsonContent = "")
		{
			var count = new Count(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return count.PostCount<T>(jsonContent).PayloadResult;
		}

		/// <summary>
		/// Count to amount of hits for a index, type and query.
		/// </summary>
		/// <typeparam name="T">Type to find</typeparam>
		/// <param name="jsonContent">json query data, returns all in emtpy</param>
		/// <returns>Result amount of document found in a result details task</returns>
		public async Task<ResultDetails<long>> CountAsync<T>(string jsonContent = "")
		{
			var count = new Count(
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
