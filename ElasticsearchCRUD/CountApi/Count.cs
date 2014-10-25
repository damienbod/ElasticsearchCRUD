using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.CountApi
{
	public class Count
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public Count(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public async Task<ResultDetails<long>> PostCountAsync<T>(string jsonContent)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for Count: {0}, content: {1}", typeof(T), jsonContent, "Count");
			var resultDetails = new ResultDetails<long>
			{
				Status = HttpStatusCode.InternalServerError,
				RequestBody = jsonContent
			};

			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/_count", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));

				var content = new StringContent(jsonContent);
				var uri = new Uri(elasticsearchUrlForEntityGet);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "Count");

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				resultDetails.RequestUrl = elasticsearchUrlForEntityGet;
				var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: GetCountAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "Count");
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
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "Count");
				var responseObject = JObject.Parse(responseString);

				resultDetails.TotalHits = (long)responseObject["count"];
				resultDetails.PayloadResult = resultDetails.TotalHits;

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "Count");
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
					_traceProvider.Trace(TraceEventType.Warning, "Count: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("Count: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "Count: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("Count: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} Count {0}, {1}", typeof(T), jsonContent, "Count");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for Count {0}, Type {1}", jsonContent, typeof(T), "Count");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for Count {0}, Type {1}", jsonContent, typeof(T), "Count"));
		}

	}
}
