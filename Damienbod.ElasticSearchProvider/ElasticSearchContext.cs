using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Damienbod.ElasticSearchProvider
{
	public class ElasticSearchContext<T> : IDisposable where T : class
	{
		private readonly ElasticSearchSerializerMapping<T> _elasticSearchSerializerMapping;

		private const string BatchOperationPath = "/_bulk";
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Uri _elasticsearchUrlBatch;
		private readonly string _elasticsearchUrlForEntityGet;
		private readonly string  _index;
		private readonly HttpClient _client = new HttpClient();

		private readonly List<Tuple<EntityContextInfo, T>>  _entityPendingChanges = new List<Tuple<EntityContextInfo, T>>();

		public ITraceProvider TraceProvider = new NullTraceProvider();

		public ElasticSearchContext(string connectionString, string index, ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping )
		{
			_index = index;
            _elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
			_elasticSearchSerializerMapping = elasticSearchSerializerMapping;
			_elasticsearchUrlForEntityGet = string.Format("{0}{1}/{2}/", connectionString, index, _elasticSearchSerializerMapping.GetDocumentType(typeof(T)));
        }

		public ElasticSearchContext(string connectionString, string index)
		{
			TraceProvider.Trace(string.Format("new ElasticSearchContext with connection string: {0}", connectionString));
			_index = index;
			_elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
			_elasticSearchSerializerMapping = new ElasticSearchSerializerMapping<T>();
			_elasticsearchUrlForEntityGet = string.Format("{0}{1}/{2}/", connectionString, index, _elasticSearchSerializerMapping.GetDocumentType(typeof(T)));
		}


		public void AddUpdateEntity(T entity, string id)
		{
			TraceProvider.Trace(string.Format("Adding entity: {0}, {1} to pending list", entity.GetType().Name, id ));
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo{Id=id,Index = _index}, entity));
		}

		public void DeleteEntity(string id)
		{
			TraceProvider.Trace(string.Format("Request to delete entity with id: {0}, Type: {1}, adding to pending list", id, typeof(T)));
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo { Id = id, Index = _index, DeleteEntity = true}, null));
		}

		public async Task<ResultDetails<T>> SaveChangesAsync()
		{
			TraceProvider.Trace("Save changes to Elasticsearch started");
			var resultDetails = new ResultDetails<T> { Status = HttpStatusCode.InternalServerError };
			try
			{
				string serializedEntities;
				using (var serializer = new ElasticsearchSerializer<T>(_elasticSearchSerializerMapping))
				{
					serializedEntities = serializer.Serialize(_entityPendingChanges);
				}
				var content = new StringContent(serializedEntities);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await _client.PostAsync(_elasticsearchUrlBatch, content, _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}

					return resultDetails;
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				resultDetails.Description = responseString;
				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				TraceProvider.Trace(string.Format("Get Request OperationCanceledException: {0}", oex.Message));
				resultDetails.Description = "OperationCanceledException";
				return resultDetails;
			}

			finally
			{
				_entityPendingChanges.Clear();
			}
		}

		public async Task<ResultDetails<T>> GetEntity(string entityId)
		{
			TraceProvider.Trace(string.Format("Request for select entity with id: {0}, Type: {1}", entityId, typeof(T)));
			var resultDetails = new ResultDetails<T>{Status=HttpStatusCode.InternalServerError};
			try
			{
				var uri = new Uri(_elasticsearchUrlForEntityGet + entityId);
				var response = await _client.GetAsync(uri,  _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				TraceProvider.Trace(string.Format("Get Request response: {0}", responseString));
				var responseObject = JObject.Parse(responseString);

				var source = responseObject["_source"];
				if (source != null)
				{
					var result = _elasticSearchSerializerMapping.ParseEntity(source);
					resultDetails.PayloadResult = result;
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				TraceProvider.Trace(string.Format("Get Request OperationCanceledException: {0}", oex.Message));
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
