using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextWarmers
{
	class ElasticsearchContextWarmer
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextWarmer(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public bool SendWarmerCommand(string contentJson, string index, string type, string warmerName)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.Execute(() => SendWarmerCommandAsync(contentJson, index, type, warmerName));	
		}

		public async Task<ResultDetails<bool>> SendWarmerCommandAsync(string contentJson, string index, string type, string warmerName)
		{
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextWarmer: Creating Warmer {0}", contentJson));

			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			var elasticsearchUrl = string.Format("{0}/_warmer/{1}/{2}/{3}", _connectionString, index, type, warmerName);
			var uri = new Uri(elasticsearchUrl);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT uri: {0}", uri.AbsoluteUri, "ElasticsearchContextWarmer");

			var content = new StringContent(contentJson);
			var response = await _client.PutAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextWarmer: Cound Not Execute Alias {0}", contentJson));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextWarmer: Cound Not Execute Alias  {0}", contentJson));
		}
	}
}
