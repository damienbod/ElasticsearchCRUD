using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContentExists
{
	public class ElasticsearchContextExists
	{
		private readonly ITraceProvider _traceProvider;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly string _connectionString;
		private readonly Exists _exists;

		public ElasticsearchContextExists(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
			ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_connectionString = connectionString;
			_exists = new Exists(_traceProvider, cancellationTokenSource, client);
		}

		public async Task<ResultDetails<bool>> DocumentExistsAsync<T>(object entityId, object parentId)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "ElasticsearchContextExists: Request for exist document with id: {0}, Type: {1}", entityId, typeof(T));

			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(typeof (T));
			var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/", _connectionString,
				elasticSearchMapping.GetIndexForType(typeof (T)), elasticSearchMapping.GetDocumentType(typeof (T)));

			string parentIdUrl = "";
			if (parentId != null)
			{
				parentIdUrl = "?parent=" + parentId;
			}
			var uri = new Uri(elasticsearchUrlForEntityGet + entityId + parentIdUrl);
			return await _exists.ExistsAsync(uri);
		}

		public bool DocumentExists<T>(object entityId, object parentId)
		{
			try
			{
				Task<ResultDetails<bool>> task = Task.Run(() => DocumentExistsAsync<T>(entityId, parentId));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Information, "ElasticsearchContextExists: DocumentExists HttpStatusCode.NotFound");
				}

				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{2} DocumentExists {0}, {1}", typeof(T), entityId, "ElasticsearchContextExists");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for DocumentExists {0}, Type {1}", entityId, typeof(T), "ElasticsearchContextExists");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for DocumentExists {0}, Type {1}", entityId, typeof(T), "ElasticsearchContextExists"));
		}
	}
}
