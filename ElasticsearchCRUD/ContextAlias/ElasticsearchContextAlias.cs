using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAlias
{
	class ElasticsearchContextAlias
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextAlias(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public bool SendAliasCommand(string contentJson)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.Execute(() => SendAliasCommandAsync(contentJson));	
		}

		public async Task<ResultDetails<bool>> SendAliasCommandAsync(string contentJson)
		{
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextAlias: Creating Alias {0}", contentJson));

			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			var elasticsearchUrlForClearCache = string.Format("{0}/_aliases", _connectionString);
			var uri = new Uri(elasticsearchUrlForClearCache);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri, "ElasticsearchContextAlias");

			var content = new StringContent(contentJson);
			var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextAlias: Cound Not Execute Alias {0}", contentJson));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: Cound Not Execute Alias  {0}", contentJson));
		}
	}
}
