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
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD
{
	public class ElasticSearchContext : IDisposable 
	{	
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly HttpClient _client = new HttpClient();
		private readonly List<EntityContextInfo> _entityPendingChanges = new List<EntityContextInfo>();	
		private readonly string _connectionString;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		public ITraceProvider TraceProvider = new NullTraceProvider();

		public bool AllowDeleteForIndex { get; set; }

		public ElasticSearchContext(string connectionString, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticSearchContext with connection string: {0}", connectionString, "ElasticSearchContext");
		}

		public ElasticSearchContext(string connectionString, IElasticSearchMappingResolver elasticSearchMappingResolver)
		{
			_elasticsearchSerializerConfiguration = new ElasticsearchSerializerConfiguration(elasticSearchMappingResolver);
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticSearchContext with connection string: {0}", connectionString, "ElasticSearchContext");
		}

		public void AddUpdateDocument(object entity, object id, object parentId = null)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Adding document: {0}, {1} to pending list", entity.GetType().Name, id, "ElasticSearchContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), EntityType = entity.GetType(), Document = entity, ParentId = parentId });
		}

		public void DeleteDocument<T>(object id)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Request to delete document with id: {0}, Type: {1}, adding to pending list", id, typeof(T).Name, "ElasticSearchContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), DeleteDocument = true, EntityType = typeof(T), Document = null});
		}

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

		public T GetDocument<T>(object entityId, object parentId = null)
		{
			var elasticsearchContextGet = new ElasticsearchContextGet(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextGet.GetDocument<T>(entityId, parentId);
		}

		public T SearchById<T>(object entityId)
		{
			var elasticsearchContextSearch = new ElasticsearchContextSearch(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextSearch.SearchById<T>(entityId);
		}

		public Collection<T> SearchForChildDocumentsByParentId<T>(object parentId, string parentDocumentType)
		{
			var elasticsearchContextSearch = new ElasticsearchContextSearch(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return elasticsearchContextSearch.SearchForChildDocumentsByParentId<T>(parentId, parentDocumentType);
		}

		public async Task<ResultDetails<Collection<T>>> SearchForChildDocumentsByParentIdAsync<T>(object parentId, string parentDocumentType)
		{
			var elasticsearchContextSearch = new ElasticsearchContextSearch(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextSearch.SearchForChildDocumentsByParentIdAsync<T>(parentId, parentDocumentType);
		}

		public async Task<ResultDetails<T>> SearchByIdAsync<T>(object entityId)
		{
			var elasticsearchContextSearch = new ElasticsearchContextSearch(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextSearch.SearchByIdAsync<T>(entityId);
		}

		public async Task<ResultDetails<T>> GetEntityAsync<T>(object entityId, object parentId = null)
		{
			var elasticsearchContextGet = new ElasticsearchContextGet(
				TraceProvider,
				_cancellationTokenSource,
				_elasticsearchSerializerConfiguration,
				_client,
				_connectionString
				);

			return await elasticsearchContextGet.GetDocumentAsync<T>(entityId, parentId);

		}

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

		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
			}
		}
	}
}
