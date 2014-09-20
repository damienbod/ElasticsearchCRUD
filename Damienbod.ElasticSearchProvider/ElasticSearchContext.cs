using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
		
		public ElasticSearchContext(string connectionString, string index, ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping)
		{
			_index = index;
            _elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
			_elasticsearchUrlForEntityGet = connectionString + index + "/" + typeof (T) + "/";
			_elasticSearchSerializerMapping = elasticSearchSerializerMapping;
        }

		public void AddUpdateEntity(T entity, string id)
		{
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo{Id=id,Index = _index}, entity));
		}

		public void DeleteEntity(string id)
		{
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo { Id = id, Index = _index, DeleteEntity = true}, null));
		}

		public async Task<ResultDetails<T>> SaveChangesAsync()
		{
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
				var responseObject = JObject.Parse(responseString);

				var items = responseObject["items"] as JArray;
				if (items != null)
				{
					var createCount = items.Count(t => t["create"]["status"].Value<int>().Equals(201));
					resultDetails.Description = createCount.ToString(CultureInfo.InvariantCulture);
				}
				return resultDetails;
			}
			catch (OperationCanceledException)
			{
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
				var responseObject = JObject.Parse(responseString);

				var source = responseObject["_source"];
				if (source != null)
				{
					var result = _elasticSearchSerializerMapping.ParseEntity(source);
					resultDetails.PayloadResult = result;
				}

				return resultDetails;
			}
			catch (OperationCanceledException)
			{
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
