using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextGet
{
	public class ElasticsearchContextGet
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextGet(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public T GetEntity<T>(object entityId)
		{
			try
			{
				var task = Task.Run(() => GetEntityAsync<T>(entityId));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticSearchContextGet: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticSearchContextGet: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticSearchContextGet: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticSearchContextGet: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} GetEntity {0}, {1}", typeof(T), entityId, "ElasticSearchContextGet");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for GetEntity {0}, Type {1}", entityId, typeof(T), "ElasticSearchContextGet");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for GetEntity {0}, Type {1}", entityId, typeof(T), "ElasticSearchContextGet"));
		}

		public async Task<ResultDetails<T>> GetEntityAsync<T>(object entityId)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for select entity with id: {0}, Type: {1}", entityId, typeof(T), "ElasticSearchContextGet");
			var resultDetails = new ResultDetails<T> { Status = HttpStatusCode.InternalServerError };
			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));
	
				var uri = new Uri(elasticsearchUrlForEntityGet + entityId);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "ElasticSearchContextGet");
				var response = await _client.GetAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: GetEntityAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticSearchContextGet");
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
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "ElasticSearchContextGet");
				var responseObject = JObject.Parse(responseString);

				var source = responseObject["_source"];
				if (source != null)
				{
					var result = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T)).ParseEntity(source, typeof(T));
					resultDetails.PayloadResult = (T)result;
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticSearchContextGet");
				return resultDetails;
			}
		}

		public async Task<ResultDetails<T>> GetChildEntityAsync<T>(object entityId, object parentId)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
			var index = elasticSearchMapping.GetIndexForType(typeof (T));
			var type = elasticSearchMapping.GetDocumentType(typeof (T));
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextGet: Searching for document id: {0}, parentId: {1}, index {2}, type {3}", entityId, parentId, index, type));

			var resultDetails = new ResultDetails<T> { Status = HttpStatusCode.InternalServerError };
			var search = new Search.Search(_traceProvider,_cancellationTokenSource, _elasticsearchSerializerConfiguration,_client,_connectionString);
			var result =  await search.PostSearchAsync<T>(BuildGetChildSearch<T>(entityId, type,parentId));
			if (result.Status == HttpStatusCode.OK && result.PayloadResult.Count > 0)
			{
				resultDetails.PayloadResult = result.PayloadResult.First();
				return resultDetails;
			}

			if (result.Status == HttpStatusCode.OK && result.PayloadResult.Count == 0)
			{
				resultDetails.Status = HttpStatusCode.NotFound;
				resultDetails.Description = string.Format("No document found id: {0}, parentId: {1}, index {3}, type {4}");
				_traceProvider.Trace(TraceEventType.Information, string.Format("ElasticsearchContextGet: No document found id: {0}, parentId: {1}, index {2}, type {3}", entityId, parentId, index, type));

				return resultDetails;
			}

			resultDetails.Status = result.Status;
			resultDetails.Description = result.Description;
			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextGet: No document found id: {0}, parentId: {1}, index {2}, type {3}", entityId, parentId, index, type));
			return resultDetails;
		}

		//		{
		//  "query": {
		//	"filtered": {
		//	  "query": {"match_all": {}},
		//	  "filter": 
		//		"and": [
		//		  {"term": {"id": 46}},
		//		  {
		//			"has_parent": {
		//			  "type": "childdocumentlevelone",
		//			  "query": {
		//				"term": {"id": "21"}
		//			  }
		//			}
		//		  }
		//		]
		//	  }
		//	}
		//  }
		//}
		private string BuildGetChildSearch<T>(object childId, string childDocumentType, object parentId)
		{
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");

			buildJson.AppendLine("\"filtered\": {");
			buildJson.AppendLine("\"query\": {\"match_all\": {}},");
			buildJson.AppendLine("\"filter\": ");
			buildJson.AppendLine("\"and\": [");
			buildJson.AppendLine("{\"term\": {\"id\": " + childId + "}},");
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"has_parent\": {");
			buildJson.AppendLine("\"type\": \""+ childDocumentType +"\",");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"term\": {\"id\": \"" + parentId + "\"}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("]");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();

		}
	}
}
