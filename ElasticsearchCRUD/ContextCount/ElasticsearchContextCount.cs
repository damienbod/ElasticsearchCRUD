using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextCount
{
	public class ElasticsearchContextCount
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextCount(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public async Task<ResultDetails<long>> PostCountAsync<T>(string jsonContent)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for ElasticsearchContextCount: {0}, content: {1}", typeof(T), jsonContent, "ElasticsearchContextCount");
			var resultDetails = new ResultDetails<long>
			{
				Status = HttpStatusCode.InternalServerError,
				RequestBody = jsonContent
			};

			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/_count", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));

				var content = new StringContent(jsonContent);
				var uri = new Uri(elasticsearchUrlForEntityGet);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "ElasticsearchContextCount");

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				resultDetails.RequestUrl = elasticsearchUrlForEntityGet;
				var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: GetCountAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticsearchContextCount");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
						resultDetails.Description = errorInfo;
						if (errorInfo.Contains("RoutingMissingException"))
						{
							throw new ElasticsearchCrudException("HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");
						}

						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "ElasticsearchContextCount");
				var responseObject = JObject.Parse(responseString);

				resultDetails.TotalHits = (long)responseObject["count"];
				resultDetails.PayloadResult = resultDetails.TotalHits;

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticsearchContextCount");
				return resultDetails;
			}
		}

		public ResultDetails<long> PostCount<T>(string jsonContent)
		{
			try
			{
				Task<ResultDetails<long>> task = Task.Run(() => PostCountAsync<T>(jsonContent));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextCount: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticsearchContextCount: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextCount: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticsearchContextCount: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} ElasticsearchContextCount {0}, {1}", typeof(T), jsonContent, "ElasticsearchContextCount");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for ElasticsearchContextCount {0}, Type {1}", jsonContent, typeof(T), "ElasticsearchContextCount");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for ElasticsearchContextCount {0}, Type {1}", jsonContent, typeof(T), "ElasticsearchContextCount"));
		}

	}
}
