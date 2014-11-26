using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate
{
	public class ElasticsearchContextIndexMapping
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextIndexMapping(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
			ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public ResultDetails<string> CreateIndexWithMapping<T>(IndexDefinition indexDefinition)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => CreateIndexWithMappingAsync<T>(indexDefinition));
		}

		public async Task<ResultDetails<string>> CreateIndexWithMappingAsync<T>(IndexDefinition indexDefinition)
		{
			if (indexDefinition == null)
			{
				indexDefinition = new IndexDefinition();
			}
			_traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateIndex Elasticsearch started", "ElasticsearchContextIndexMapping");
			var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

			try
			{
				var item = Activator.CreateInstance<T>();
				var entityContextInfo = new EntityContextInfo
				{
					RoutingDefinition = indexDefinition.RoutingDefinition,
					Document = item,
					EntityType = typeof (T),
					Id = "0"
				};

				string index =
					_elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(
						entityContextInfo.EntityType).GetIndexForType(entityContextInfo.EntityType);
				MappingUtils.GuardAgainstBadIndexName(index);

				var indexMappings = new IndexMappings(_traceProvider, _elasticsearchSerializerConfiguration);
				indexMappings.CreateIndexSettingsForDocument(index, indexDefinition.IndexSettings);
				indexMappings.CreatePropertyMappingForTopDocument(entityContextInfo, index);
				await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message,
					"ElasticsearchContextIndexMapping");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}
		}

		public ResultDetails<string> CreateIndex(string index, IndexSettings indexSettings)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => CreateIndexAsync(index, indexSettings));
		}

		public async Task<ResultDetails<string>> CreateIndexAsync(string index, IndexSettings indexSettings)
		{
			if (string.IsNullOrEmpty(index))
			{
				throw new ElasticsearchCrudException("CreateIndexAsync: index is required");
			}
			if (indexSettings == null)
			{
				indexSettings = new IndexSettings{NumberOfShards=5,NumberOfReplicas=1};
			}

			_traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateIndex Elasticsearch started", "ElasticsearchContextIndexMapping");
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };

			try
			{
				MappingUtils.GuardAgainstBadIndexName(index);

				var indexMappings = new IndexMappings(_traceProvider, _elasticsearchSerializerConfiguration);
				indexMappings.CreateIndexSettingsForDocument(index, indexSettings);
				await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message,
					"ElasticsearchContextIndexMapping");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}
		}

		public ResultDetails<string> CreateTypeMappingForIndex<T>(MappingDefinition mappingDefinition)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => CreateTypeMappingForIndexAsync<T>(mappingDefinition));
		}

		public async Task<ResultDetails<string>> CreateTypeMappingForIndexAsync<T>(MappingDefinition mappingDefinition)
		{
			if (mappingDefinition == null)
			{
				throw new ElasticsearchCrudException("CreateTypeMappingForIndexAsync: A mapping definition with the parent index is required");
			}
			_traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateTypeMappingForIndex Elasticsearch started", "ElasticsearchContextIndexMapping");
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };

			try
			{
				var indexMappings = new IndexMappings(_traceProvider, _elasticsearchSerializerConfiguration);

				var item = Activator.CreateInstance<T>();
				var entityContextInfo = new EntityContextInfo
				{
					RoutingDefinition = mappingDefinition.RoutingDefinition,
					Document = item,
					EntityType = typeof (T),
					Id = "0"
				};
				indexMappings.CreatePropertyMappingForTopDocument(entityContextInfo, mappingDefinition.Index);
				await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: CreateTypeMappingForIndexAsync Request OperationCanceledException: {0}", oex.Message,
					"ElasticsearchContextIndexMapping");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}
		}

		public ResultDetails<string> UpdateSettings(string index, IndexSettings indexSettings)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => UpdateSettingsAsync(index, indexSettings));
		}

		public async Task<ResultDetails<string>> UpdateSettingsAsync(string index, IndexSettings indexSettings)
		{
			if (indexSettings == null)
			{
				indexSettings = new IndexSettings { NumberOfShards = 5, NumberOfReplicas = 1 };
			}

			_traceProvider.Trace(TraceEventType.Verbose, "{0}: UpdateSettingsAsync Elasticsearch started", "ElasticsearchContextIndexMapping");
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };

			try
			{

				var indexMappings = new IndexMappings(_traceProvider, _elasticsearchSerializerConfiguration);
				indexMappings.UpdateSettings(index, indexSettings);
				await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message,
					"ElasticsearchContextIndexMapping");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}
		}
	}
}
