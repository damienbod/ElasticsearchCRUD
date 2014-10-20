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

namespace ElasticsearchCRUD.Search
{
	public class Search
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public Search(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public async Task<ResultDetails<Collection<T>>> PostSearchAsync<T>(string jsonContent)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for search: {0}, content: {1}", typeof(T), jsonContent, "Search");
			var resultDetails = new ResultDetails<Collection<T>> { Status = HttpStatusCode.InternalServerError };
			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/_search", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));

				var content = new StringContent(jsonContent);
				var uri = new Uri(elasticsearchUrlForEntityGet);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "Search");

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);
				
				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: GetSearchAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "Search");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						if (errorInfo.Contains("RoutingMissingException"))
						{
							throw new ElasticsearchCrudException("HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");
						}

						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "Search");
				var responseObject = JObject.Parse(responseString);

				var source = responseObject["_source"];
				if (source != null)
				{
					var result = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T)).ParseEntity(source, typeof(T));
					//resultDetails.PayloadResult = (T)result;
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "Search");
				return resultDetails;
			}
		}

	}
}
