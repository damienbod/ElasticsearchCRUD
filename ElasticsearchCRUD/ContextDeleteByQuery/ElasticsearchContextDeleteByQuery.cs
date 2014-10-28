using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextDeleteByQuery
{
	public class ElasticsearchContextDeleteByQuery
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextDeleteByQuery(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public async Task<ResultDetails<bool>> DeleteByQueryAsync<T>(string jsonContent)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for ElasticsearchContextDeleteByQuery: {0}, content: {1}", typeof(T), jsonContent, "ElasticsearchContextDeleteByQuery");
			var resultDetails = new ResultDetails<bool>
			{
				Status = HttpStatusCode.InternalServerError,
				RequestBody = jsonContent
			};

			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/_query", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));	
				
				var content = new StringContent(jsonContent);
				var uri = new Uri(elasticsearchUrlForEntityGet);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP DELETE uri: {0}", uri.AbsoluteUri, "Search");

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				var request = new HttpRequestMessage(HttpMethod.Delete, uri)
				{
					Content = content
				};
				var response = await _client.SendAsync(request,_cancellationTokenSource.Token).ConfigureAwait(false); ;


				resultDetails.RequestUrl = elasticsearchUrlForEntityGet;
				//var response = await _client.DeleteAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);
				
				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: GetSearchAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "Search");
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
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "Search");
				var responseObject = JObject.Parse(responseString);

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "Search");
				return resultDetails;
			}
		}

		public ResultDetails<bool> SendDeleteByQuery<T>(string jsonContent)
		{
			try
			{
				Task<ResultDetails<bool>> task = Task.Run(() => DeleteByQueryAsync<T>(jsonContent));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextDeleteByQuery: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticsearchContextDeleteByQuery: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextDeleteByQuery: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticsearchContextDeleteByQuery: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} Search {0}, {1}", typeof(T), jsonContent, "ElasticsearchContextDeleteByQuery");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for Search {0}, Type {1}", jsonContent, typeof(T), "ElasticsearchContextDeleteByQuery");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for Search {0}, Type {1}", jsonContent, typeof(T), "ElasticsearchContextDeleteByQuery"));
		}

	}
}
