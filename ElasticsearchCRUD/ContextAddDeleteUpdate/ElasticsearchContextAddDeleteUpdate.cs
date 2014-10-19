using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate
{
	public class ElasticsearchContextAddDeleteUpdate
	{
		private const string BatchOperationPath = "/_bulk";
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;
		private bool _saveChangesAndInitMappingsForChildDocuments;
		private Uri _elasticsearchUrlBatch;

		public ElasticsearchContextAddDeleteUpdate(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
			_elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
		}

		public ResultDetails<string> SaveChanges(List<EntityContextInfo> entityPendingChanges, bool saveChangesAndInitMappingsForChildDocuments)
		{
			_saveChangesAndInitMappingsForChildDocuments = saveChangesAndInitMappingsForChildDocuments;
			try
			{
				var task = Task.Run(() => SaveChangesAsync(entityPendingChanges));
				task.Wait();
				if (!string.IsNullOrEmpty(task.Result.Description))
				{
					_traceProvider.Trace(TraceEventType.Warning, "{0}: SaveChanges {1}", "ElasticsearchContextAddDeleteUpdate", task.Result.Description);
				}

				return task.Result;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{0}: SaveChanges", "ElasticsearchContextAddDeleteUpdate");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "{0}: Unknown error for SaveChanges", "ElasticsearchContextAddDeleteUpdate");
			throw new ElasticsearchCrudException("ElasticSearchContext: Unknown error for SaveChanges");
		}

		public async Task<ResultDetails<string>> SaveChangesAsync(List<EntityContextInfo> entityPendingChanges)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{0}: Save changes to Elasticsearch started", "ElasticsearchContextAddDeleteUpdate");
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };

			if (entityPendingChanges.Count == 0)
			{
				resultDetails = new ResultDetails<string> { Status = HttpStatusCode.OK, Description = "Nothing to save" };
				return resultDetails;
			}

			try
			{
				string serializedEntities;
				using (var serializer = new ElasticsearchSerializer(_traceProvider, _elasticsearchSerializerConfiguration))
				{

					var result = serializer.Serialize(entityPendingChanges);
					if (_saveChangesAndInitMappingsForChildDocuments)
					{
						await result.InitMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);
						_saveChangesAndInitMappingsForChildDocuments = false;
					}
					serializedEntities = result.Content;
				}
				var content = new StringContent(serializedEntities);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: sending bulk request: {0}", serializedEntities, "ElasticsearchContextAddDeleteUpdate");
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", _elasticsearchUrlBatch.AbsoluteUri, "ElasticsearchContextAddDeleteUpdate");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await _client.PostAsync(_elasticsearchUrlBatch, content, _cancellationTokenSource.Token).ConfigureAwait(true);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: SaveChangesAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticsearchContextAddDeleteUpdate");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}

					return resultDetails;
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				var responseObject = JObject.Parse(responseString);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: response: {0}", responseString, "ElasticsearchContextAddDeleteUpdate");
				string errors = String.Empty;
				var items = responseObject["items"];
				if (items != null)
				{
					foreach (var item in items)
					{
						if (item["delete"] != null && item["delete"]["status"] != null)
						{
							if (item["delete"]["status"].ToString() == "404")
							{
								resultDetails.Status = HttpStatusCode.NotFound;
								errors = errors + string.Format("Delete failed for item: {0}, {1}, {2}  :", item["delete"]["_index"],
									item["delete"]["_type"], item["delete"]["_id"]);
							}
						}
					}
				}

				resultDetails.Description = responseString;
				resultDetails.PayloadResult = serializedEntities;

				if (!String.IsNullOrEmpty(errors))
				{
					_traceProvider.Trace(TraceEventType.Warning, errors);
					throw new ElasticsearchCrudException(errors);
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticsearchContextAddDeleteUpdate");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}

			finally
			{
				entityPendingChanges.Clear();
			}
		}

		public async Task<ResultDetails<T>> DeleteIndexAsync<T>(bool allowDeleteForIndex)
		{
			if (!allowDeleteForIndex)
			{
				_traceProvider.Trace(TraceEventType.Error, "{0}: Delete Index is not activated for this context. If ou want to activate it, set the AllowDeleteForIndex property of the context", "ElasticsearchContextAddDeleteUpdate");
				throw new ElasticsearchCrudException("ElasticSearchContext: Delete Index is not activated for this context. If ou want to activate it, set the AllowDeleteForIndex property of the context");
			}
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request to delete complete index for Type: {0}", typeof(T), "ElasticsearchContextAddDeleteUpdate");

			var resultDetails = new ResultDetails<T> { Status = HttpStatusCode.InternalServerError };
			try
			{
				var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForIndexDelete = string.Format("{0}{1}", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)));
				var uri = new Uri(elasticsearchUrlForIndexDelete);
				_traceProvider.Trace(TraceEventType.Warning, "{1}: Request HTTP Delete uri: {0}", uri.AbsoluteUri, "ElasticsearchContextAddDeleteUpdate");
				var response = await _client.DeleteAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: DeleteIndexAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticsearchContextAddDeleteUpdate");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Delete Index Request response: {0}", responseString, "ElasticsearchContextAddDeleteUpdate");
				resultDetails.Description = responseString;

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticsearchContextAddDeleteUpdate");
				return resultDetails;
			}
		}

	}
}
