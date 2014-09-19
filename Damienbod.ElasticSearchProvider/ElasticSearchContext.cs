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

		private const string BulkServiceOperationPath = "/_bulk";
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Uri _elasticsearchUrl;

		private readonly List<Tuple<EntityContextInfo, T>>  _entityPendingChanges = new List<Tuple<EntityContextInfo, T>>(); 
		
		public ElasticSearchContext(string connectionString, ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping)
        {
            _elasticsearchUrl = new Uri(new Uri(connectionString), BulkServiceOperationPath);
			_elasticSearchSerializerMapping = elasticSearchSerializerMapping;
        }

		public void AddUpdateEntity(T entity, string id, string index)
		{
			_entityPendingChanges.Add(new Tuple<EntityContextInfo, T>(new EntityContextInfo(){Id=id,Index = index}, entity));
		}

		public async Task<int> SaveChangesAsync()
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
				var response = await client.PostAsync(_elasticsearchUrl, content, _cancellationTokenSource.Token).ConfigureAwait(false);

				if (response.StatusCode != HttpStatusCode.OK)
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var messagesDiscarded = _entityPendingChanges.Count();
						var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						string serverErrorMessage;

						try
						{
							var errorObject = JObject.Parse(errorContent);
							serverErrorMessage = errorObject["error"].Value<string>();
						}
						catch (Exception)
						{
							serverErrorMessage = errorContent;
						}

						// We are unable to write the batch
						// log here

						return messagesDiscarded;
					}

					return 0;
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				var responseObject = JObject.Parse(responseString);

				var items = responseObject["items"] as JArray;

				// If the response return items collection
				if (items != null)
				{
					return items.Count(t => t["create"]["status"].Value<int>().Equals(201));
				}

				return 0;
			}
			catch (OperationCanceledException)
			{
				return 0;
			}

			finally
			{
				if (client != null)
				{
					client.Dispose();
				}
			}
		}
	}
}
