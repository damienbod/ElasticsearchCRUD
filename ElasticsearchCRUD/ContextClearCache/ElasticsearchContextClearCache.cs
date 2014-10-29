using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContextClearCache
{
	public class ElasticsearchContextClearCache
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextClearCache(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public bool ClearCacheForIndex<T>()
		{
			try
			{
				Task<ResultDetails<bool>> task = Task.Run(() => ClearCacheForIndexAsync<T>());
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextClearCache: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticsearchContextClearCache: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextClearCache: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticsearchContextClearCache: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} SearchById {0}, {1}", typeof(T), "ElasticsearchContextClearCache");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "ElasticsearchContextClearCache: ClearCacheForIndex {0}", typeof(T));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextClearCache: ClearCacheForIndex {0}", typeof(T)));
		}

		public async Task<ResultDetails<bool>> ClearCacheForIndexAsync<T>()
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof(T));
			var index = elasticSearchMapping.GetIndexForType(typeof(T));
			var type = elasticSearchMapping.GetDocumentType(typeof(T));
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextClearCache: Clearing Cache for index {0}", index));

			var resultDetails = new ResultDetails<bool> {Status = HttpStatusCode.InternalServerError};
			var elasticsearchUrlForClearCache = string.Format("{0}/{1}/_cache/clear", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)));
			var uri = new Uri(elasticsearchUrlForClearCache);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri, "ElasticsearchContextClearCache");

			var request = new HttpRequestMessage(HttpMethod.Post, uri);
			var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(false); ;

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextClearCache: Could nor clear cache for index {0}", elasticSearchMapping.GetIndexForType(typeof(T))));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextClearCache: Could nor clear cache for index {0}", elasticSearchMapping.GetIndexForType(typeof(T))));
		}
	}
}
