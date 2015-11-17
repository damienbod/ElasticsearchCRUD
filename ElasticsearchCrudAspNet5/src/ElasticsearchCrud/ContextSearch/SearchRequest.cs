using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch
{
	public class SearchRequest
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public SearchRequest(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public async Task<ResultDetails<SearchResult<T>>> PostSearchAsync<T>(string jsonContent, string scrollId, ScanAndScrollConfiguration scanAndScrollConfiguration, SearchUrlParameters searchUrlParameters)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for search: {0}, content: {1}", typeof(T), jsonContent, "Search");

			var urlParams = "";
			if (searchUrlParameters != null)
			{
				urlParams = searchUrlParameters.GetUrlParameters();
			}
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof(T));
			var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/_search{3}", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)), urlParams);

			if (!string.IsNullOrEmpty(scrollId))
			{
				elasticsearchUrlForEntityGet = string.Format("{0}/{1}{2}", _connectionString ,scanAndScrollConfiguration.GetScrollScanUrlForRunning(),scrollId);
			}

			var uri = new Uri(elasticsearchUrlForEntityGet);

			var result = await PostInteranlSearchAsync<T>(jsonContent, uri);
			return result;
		}

		public ResultDetails<SearchResult<T>> PostSearch<T>(string jsonContent, string scrollId, ScanAndScrollConfiguration scanAndScrollConfiguration, SearchUrlParameters searchUrlParameters)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => PostSearchAsync<T>(jsonContent, scrollId, scanAndScrollConfiguration, searchUrlParameters));
		}

		public async Task<ResultDetails<SearchResult<T>>> PostSearchCreateScanAndScrollAsync<T>(string jsonContent, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for search create scan ans scroll: {0}, content: {1}", typeof (T), jsonContent, "Search");

			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof (T));
			var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/_search", _connectionString,
				elasticSearchMapping.GetIndexForType(typeof (T)), elasticSearchMapping.GetDocumentType(typeof (T)));

			elasticsearchUrlForEntityGet = elasticsearchUrlForEntityGet + "?" + scanAndScrollConfiguration.GetScrollScanUrlForSetup();

			var uri = new Uri(elasticsearchUrlForEntityGet);
			var result = await PostInteranlSearchAsync<T>(jsonContent, uri);
			return result;
		}

		public ResultDetails<SearchResult<T>> PostSearchCreateScanAndScroll<T>(string jsonContent, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => PostSearchCreateScanAndScrollAsync<T>(jsonContent, scanAndScrollConfiguration));
		}

		public async Task<ResultDetails<bool>> PostSearchExistsAsync<T>(string jsonContent, SearchUrlParameters searchUrlParameters)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for search exists: {0}, content: {1}", typeof(T), jsonContent, "Search");
			var resultDetails = new ResultDetails<bool>
			{
				Status = HttpStatusCode.InternalServerError,
				RequestBody = jsonContent
			};

			var urlParams = "";
			if (searchUrlParameters != null)
			{
				urlParams = searchUrlParameters.GetUrlParameters();
			}

			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForSearchExists = string.Format("{0}/{1}/{2}/_search/exists{3}", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)), urlParams);

				var content = new StringContent(jsonContent);
				var uri = new Uri(elasticsearchUrlForSearchExists);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP Post uri: {0}", uri.AbsoluteUri, "Search");

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				resultDetails.RequestUrl = elasticsearchUrlForSearchExists;
				var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: Post seach exists async response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "Search");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}

					if (response.StatusCode == HttpStatusCode.NotFound)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "Search");
				var responseObject = JObject.Parse(responseString);


				var source = responseObject["exists"];

				resultDetails.PayloadResult = (bool)source;
				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "Search");
				return resultDetails;
			}
		}

		public bool PostSearchExists<T>(string jsonContent, SearchUrlParameters searchUrlParameters)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.ExecuteResultDetails(() => PostSearchExistsAsync<T>(jsonContent, searchUrlParameters)).PayloadResult;
		}

		private async Task<ResultDetails<SearchResult<T>>> PostInteranlSearchAsync<T>(string jsonContent, Uri uri)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for search: {0}, content: {1}", typeof(T), jsonContent, "Search");
			var resultDetails = new ResultDetails<SearchResult<T>>
			{
				Status = HttpStatusCode.InternalServerError,
				RequestBody = jsonContent
			};

			try
			{			
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "Search");
				var content = new StringContent(jsonContent);

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				resultDetails.RequestUrl = uri.ToString();
				var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);

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

					if (response.StatusCode == HttpStatusCode.NotFound)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "Search");
				var responseObject = JObject.Parse(responseString);
				var ser = new JsonSerializer();
				ser.Converters.Add(new GeoShapeGeometryCollectionGeometriesConverter());

				resultDetails.PayloadResult = responseObject.ToObject<SearchResult<T>>(ser);
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
