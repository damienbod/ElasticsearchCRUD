using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Damienbod.ElasticSearchProvider
{
	public class ElasticSearchContext<T> where T : class
	{
		private readonly ElasticSearchSerializerMapping<T> _elasticSearchSerializerMapping;

		private const string BatchOperationPath = "/_bulk";
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Uri _elasticsearchUrlBatch;

		private readonly List<Tuple<EntityContextInfo, T>>  _entityPendingChanges = new List<Tuple<EntityContextInfo, T>>(); 
		
		public ElasticSearchContext(string connectionString, ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping)
        {
            _elasticsearchUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
			_elasticSearchSerializerMapping = elasticSearchSerializerMapping;
        }

		public void AddUpdateEntity(T entity, string id, string index)
		{
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo(){Id=id,Index = index}, entity));
		}

		public void DeleteEntity(string id, string index)
		{
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo() { Id = id, Index = index, DeleteEntity = true}, null));
		}

		public async Task<bool> SaveChangesAsync()
		{
			HttpClient client = null;

			try
			{
				client = new HttpClient();

				string serializedEntities;
				using (var serializer = new ElasticsearchSerializer<T>(_elasticSearchSerializerMapping))
				{
					serializedEntities = serializer.Serialize(_entityPendingChanges);
				}
				var content = new StringContent(serializedEntities);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await client.PostAsync(_elasticsearchUrlBatch, content, _cancellationTokenSource.Token).ConfigureAwait(false);

				if (response.StatusCode != HttpStatusCode.OK)
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						// TODO add tracing or logging
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

						return false;
					}

					return true;
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				var responseObject = JObject.Parse(responseString);

				var items = responseObject["items"] as JArray;
				var createCount = items.Count(t => t["create"]["status"].Value<int>().Equals(201));
				// TODO log or trace

				return true;
			}
			catch (OperationCanceledException)
			{
				return true;
			}

			finally
			{
				_entityPendingChanges.Clear();
				if (client != null)
				{
					client.Dispose();
				}
			}
		}
	}
}
