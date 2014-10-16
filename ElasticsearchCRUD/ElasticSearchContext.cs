using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD
{
	public class ElasticSearchContext : IDisposable 
	{
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver;

		private const string BatchOperationPath = "/_bulk";
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Uri _elasticsearchUrlBatch;
		private readonly HttpClient _client = new HttpClient();
		private readonly List<EntityContextInfo> _entityPendingChanges = new List<EntityContextInfo>();	
		private readonly string _connectionString;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		public ITraceProvider TraceProvider = new NullTraceProvider();

		public bool AllowDeleteForIndex { get; set; }

		public ElasticSearchContext(string connectionString, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticSearchContext with connection string: {0}", connectionString, "ElasticSearchContext");
			_elasticSearchMappingResolver = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver;
			_elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
		}

		public ElasticSearchContext(string connectionString, IElasticSearchMappingResolver elasticSearchMappingResolver)
		{
			_elasticsearchSerializerConfiguration = new ElasticsearchSerializerConfiguration(elasticSearchMappingResolver);
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticSearchContext with connection string: {0}", connectionString, "ElasticSearchContext");
			_elasticSearchMappingResolver = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver;
			_elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);

		}

		public void AddUpdateEntity(object entity, object id)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Adding entity: {0}, {1} to pending list", entity.GetType().Name, id, "ElasticSearchContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), EntityType = entity.GetType() , Entity = entity });
		}

		public void DeleteEntity<T>(object id)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Request to delete entity with id: {0}, Type: {1}, adding to pending list", id, typeof(T).Name, "ElasticSearchContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), DeleteEntity = true, EntityType = typeof(T), Entity = null});
		}

		private bool _saveChangesAndInitMappingsForChildDocuments = false;
		public ResultDetails<string> SaveChangesAndInitMappingsForChildDocuments()
		{
			_saveChangesAndInitMappingsForChildDocuments = true;
			return SaveChanges();
		}

		public ResultDetails<string> SaveChanges()
		{
			try
			{
				var task = Task.Run(() => SaveChangesAsync());
				task.Wait();
				if (!string.IsNullOrEmpty(task.Result.Description))
				{
					TraceProvider.Trace(TraceEventType.Warning, "{0}: SaveChanges {1}", "ElasticSearchContext" , task.Result.Description);
				}
				
				return task.Result;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					TraceProvider.Trace(TraceEventType.Warning, x, "{0}: SaveChanges", "ElasticSearchContext");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			TraceProvider.Trace(TraceEventType.Error, "{0}: Unknown error for SaveChanges", "ElasticSearchContext");
			throw new ElasticsearchCrudException("ElasticSearchContext: Unknown error for SaveChanges");
		}

		public async Task<ResultDetails<string>> SaveChangesAsync()
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{0}: Save changes to Elasticsearch started", "ElasticSearchContext");
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };

			if (_entityPendingChanges.Count == 0)
			{
				resultDetails = new ResultDetails<string> {Status = HttpStatusCode.OK, Description = "Nothing to save"};
				return resultDetails;
			}

			try
			{
				string serializedEntities;
				using (var serializer = new ElasticsearchSerializer(TraceProvider, _elasticsearchSerializerConfiguration))
				{
					
					var result = serializer.Serialize(_entityPendingChanges);
					if (_saveChangesAndInitMappingsForChildDocuments)
					{
						var initResult = await result.InitMappings.Execute(_client, _connectionString, TraceProvider, _cancellationTokenSource);
						_saveChangesAndInitMappingsForChildDocuments = false;
					}
					serializedEntities = result.Content;
				}
				var content = new StringContent(serializedEntities);
				TraceProvider.Trace(TraceEventType.Verbose, "{1}: sending bulk request: {0}", serializedEntities, "ElasticSearchContext");
				TraceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", _elasticsearchUrlBatch.AbsoluteUri, "ElasticSearchContext");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await _client.PostAsync(_elasticsearchUrlBatch, content, _cancellationTokenSource.Token).ConfigureAwait(true);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					TraceProvider.Trace(TraceEventType.Warning, "{2}: SaveChangesAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticSearchContext");
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
				TraceProvider.Trace(TraceEventType.Verbose, "{1}: response: {0}", responseString, "ElasticSearchContext");
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
					TraceProvider.Trace(TraceEventType.Warning, errors);
					throw new ElasticsearchCrudException(errors);
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				TraceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticSearchContext");
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}

			finally
			{
				_entityPendingChanges.Clear();
			}
		}

		public T GetEntity<T>(object entityId)
		{
			try
			{
				var task = Task.Run(() => GetEntityAsync<T>(entityId));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					TraceProvider.Trace(TraceEventType.Warning, "HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("HttpStatusCode.NotFound");
				}
				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					TraceProvider.Trace(TraceEventType.Warning, x, "{2} GetEntity {0}, {1}", typeof(T), entityId, "ElasticSearchContext");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			TraceProvider.Trace(TraceEventType.Error, "{2}: Unknown error for GetEntity {0}, Type {1}", entityId, typeof(T), "ElasticSearchContext");
			throw new ElasticsearchCrudException(string.Format("{2}: Unknown error for GetEntity {0}, Type {1}", entityId, typeof(T), "ElasticSearchContext"));
		}

		public async Task<ResultDetails<T>> GetEntityAsync<T>(object entityId)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Request for select entity with id: {0}, Type: {1}", entityId, typeof(T), "ElasticSearchContext");
			var resultDetails = new ResultDetails<T>{Status=HttpStatusCode.InternalServerError};
			try
			{
				var elasticSearchMapping = _elasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForEntityGet = string.Format("{0}/{1}/{2}/", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));
				var uri = new Uri(elasticsearchUrlForEntityGet + entityId);
				TraceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "ElasticSearchContext");
				var response = await _client.GetAsync(uri,  _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					TraceProvider.Trace(TraceEventType.Warning, "{2}: GetEntityAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticSearchContext");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				TraceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "ElasticSearchContext");
				var responseObject = JObject.Parse(responseString);

				var source = responseObject["_source"];
				if (source != null)
				{
					var result = _elasticSearchMappingResolver.GetElasticSearchMapping(typeof(T)).ParseEntity(source, typeof(T));
					resultDetails.PayloadResult = (T)result;
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				TraceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticSearchContext");
				return resultDetails;
			}
		}

		public async Task<ResultDetails<T>> DeleteIndexAsync<T>()
		{
			if (!AllowDeleteForIndex)
			{
				TraceProvider.Trace(TraceEventType.Error, "{0}: Delete Index is not activated for this context. If ou want to activate it, set the AllowDeleteForIndex property of the context", "ElasticSearchContext");
				throw new ElasticsearchCrudException("ElasticSearchContext: Delete Index is not activated for this context. If ou want to activate it, set the AllowDeleteForIndex property of the context");
			}
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: Request to delete complete index for Type: {0}", typeof(T), "ElasticSearchContext");

			var resultDetails = new ResultDetails<T> { Status = HttpStatusCode.InternalServerError };
			try
			{
				var elasticSearchMapping = _elasticSearchMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticsearchUrlForIndexDelete = string.Format("{0}{1}", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)));
				var uri = new Uri(elasticsearchUrlForIndexDelete );
				TraceProvider.Trace(TraceEventType.Warning, "{1}: Request HTTP Delete uri: {0}", uri.AbsoluteUri, "ElasticSearchContext");
				var response = await _client.DeleteAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					TraceProvider.Trace(TraceEventType.Warning, "{2}: DeleteIndexAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticSearchContext");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				TraceProvider.Trace(TraceEventType.Verbose, "{1}: Delete Index Request response: {0}", responseString, "ElasticSearchContext");
				resultDetails.Description = responseString;

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				TraceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticSearchContext");
				return resultDetails;
			}
		}

		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
			}
		}
	}
}
