using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

		public ResultDetails<string> CreateIndex<T>(RoutingDefinition routingDefinition)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => CreateIndexAsync<T>(routingDefinition));
		}

		public async Task<ResultDetails<string>> CreateIndexAsync<T>(RoutingDefinition routingDefinition)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{0}: Save changes to Elasticsearch started", "ElasticsearchContextCreateIndex");
			var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

			try
			{
				using (var serializer = new ElasticsearchSerializer(_traceProvider, _elasticsearchSerializerConfiguration, true))
				{
					var item = Activator.CreateInstance<T>();
					var entityContextInfo = new EntityContextInfo
					{
						RoutingDefinition = routingDefinition,
						Document = item,
						EntityType = typeof (T),
						Id = "0"
					};

					var resultMappings = serializer.SerializeMapping(new List<EntityContextInfo> { entityContextInfo });
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

	}
}
