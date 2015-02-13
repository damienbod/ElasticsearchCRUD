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

		public bool SendWarmerCommand(Warmer warmer, string index, string type)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.Execute(() => SendWarmerCommandAsync(warmer, index, type));	
		}

		public async Task<ResultDetails<bool>> SendWarmerCommandAsync(Warmer warmer, string index, string type)
		{
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextWarmer: Creating Warmer {0}", warmer));

			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			var elasticsearchUrl = string.Format("{0}/_warmer/{1}/{2}/{3}", _connectionString, index, type, warmer.Name);
			var uri = new Uri(elasticsearchUrl);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT uri: {0}", uri.AbsoluteUri, "ElasticsearchContextWarmer");

			var content = new StringContent(warmer.ToString());
			var response = await _client.PutAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextWarmer: Cound Not Execute Alias {0}", warmer));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextWarmer: Cound Not Execute Alias  {0}", warmer));
		}
	}
}
