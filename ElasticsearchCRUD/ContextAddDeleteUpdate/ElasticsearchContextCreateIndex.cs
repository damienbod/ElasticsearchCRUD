using System;
using System.Collections.Generic;
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
	public class ElasticsearchContextCreateIndex
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextCreateIndex(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
			ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public ResultDetails<string> CreateIndex<T>(IndexDefinition indexDefinition)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => CreateIndexAsync<T>(indexDefinition));
		}

		public async Task<ResultDetails<string>> CreateIndexAsync<T>(IndexDefinition indexDefinition)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateIndex Elasticsearch started", "ElasticsearchContextCreateIndex");
			var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

			try
			{
				using (var serializer = new ElasticsearchSerializer(_traceProvider, _elasticsearchSerializerConfiguration, true))
				{
					var item = Activator.CreateInstance<T>();
					var entityContextInfo = new EntityContextInfo
					{
						RoutingDefinition = indexDefinition.RoutingDefinition,
						Document = item,
						EntityType = typeof (T),
						Id = "0"
					};

					var resultMappings = serializer.SerializeMapping(new List<EntityContextInfo> { entityContextInfo }, indexDefinition.IndexSettings);
					await resultMappings.IndexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticsearchContextCreateIndex");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}
		}

		public ResultDetails<string> TypeMappingForIndex<T>(MappingDefinition mappingDefinition)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => TypeMappingForIndexAsync<T>(mappingDefinition));
		}

		public async Task<ResultDetails<string>> TypeMappingForIndexAsync<T>(MappingDefinition mappingDefinition)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{0}: TypeMappingForIndex Elasticsearch started", "ElasticsearchContextCreateIndex");
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
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: TypeMappingForIndexAsync Request OperationCanceledException: {0}", oex.Message,
					"ElasticsearchContextCreateIndex");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}
		}

	}
}
