using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.SearchApi;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContextSearch
{
	public class ElasticsearchContextSearch
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextSearch(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public T SearchById<T>(object entityId)
		{
			try
			{
				Task<ResultDetails<T>> task = Task.Run(() => SearchByIdAsync<T>(entityId));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextSearch: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticsearchContextSearch: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextSearch: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticsearchContextSearch: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} SearchById {0}, {1}", typeof(T), entityId, "ElasticsearchContextSearch");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for SearchById {0}, Type {1}", entityId, typeof(T), "ElasticsearchContextSearch");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for SearchById {0}, Type {1}", entityId, typeof(T), "ElasticsearchContextSearch"));
		}

		public async Task<ResultDetails<T>> SearchByIdAsync<T>(object entityId)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
			var index = elasticSearchMapping.GetIndexForType(typeof(T));
			var type = elasticSearchMapping.GetDocumentType(typeof(T));
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextSearch: Searching for document id: {0}, index {1}, type {2}", entityId, index, type));

			var resultDetails = new ResultDetails<T> {Status = HttpStatusCode.InternalServerError};
			
			var search = new Search(_traceProvider, _cancellationTokenSource, _elasticsearchSerializerConfiguration, _client, _connectionString);

			var result = await search.PostSearchAsync<T>(BuildSearchById<T>(entityId));
			resultDetails.TotalHits = result.TotalHits;
			resultDetails.RequestBody = result.RequestBody;
			resultDetails.RequestUrl = result.RequestUrl;

			if (result.Status == HttpStatusCode.OK && result.PayloadResult.Count > 0)
			{
				resultDetails.PayloadResult = result.PayloadResult.First();
				return resultDetails;
			}

			if (result.Status == HttpStatusCode.OK && result.PayloadResult.Count == 0)
			{
				resultDetails.Status = HttpStatusCode.NotFound;
				resultDetails.Description = string.Format("No document found id: {0}, index {1}, type {2}", entityId, index, type);
				_traceProvider.Trace(TraceEventType.Information, string.Format("ElasticsearchContextSearch: No document found id: {0},, index {1}, type {2}", entityId, index, type));
				return resultDetails;
			}

			resultDetails.Status = result.Status;
			resultDetails.Description = result.Description;
			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextSearch: No document found id: {0},  index {1}, type {2}", entityId, index, type));
			return resultDetails;
		}

		// {
		//  "query" : {
		//	  "filtered": {
		//		"query": { 
		//         "term": {"id": "47"}
	    //       }
		//	   }
		//   }
		// }
		private string BuildSearchById<T>(object childId)
		{
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"filtered\": {");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"term\": {\"id\": " + childId + "}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}

	}
}
