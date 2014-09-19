using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Damienbod.BusinessLayer.DomainModel;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Damienbod.ElasticSearchProvider
{
	public class ElasticSearchContext
	{
		private const string BulkServiceOperationPath = "/_bulk";
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Uri _elasticsearchUrl;

		public ElasticSearchContext(string connectionString)
        {
            _elasticsearchUrl = new Uri(new Uri(connectionString), BulkServiceOperationPath);
        }

		public async Task<int> SendEntitiesAsync(IList<Animal> collection, string index)
		{
			HttpClient client = null;

			try
			{
				client = new HttpClient();

				string logMessages;
				using (var serializer = new ElasticsearchSerializer())
				{
					logMessages = serializer.Serialize(collection, index);
				}
				var content = new StringContent(logMessages);
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				var response = await client.PostAsync(this._elasticsearchUrl, content, _cancellationTokenSource.Token).ConfigureAwait(false);

				if (response.StatusCode != HttpStatusCode.OK)
				{
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var messagesDiscarded = collection.Count();
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
					// NOTE: This only works with Elasticsearch 1.0
					// Alternatively we could query ES as part of initialization check results or fall back to trying <1.0 parsing
					// We should also consider logging errors for individual entries
					return items.Count(t => t["create"]["status"].Value<int>().Equals(201));

					// Pre-1.0 Elasticsearch
					// return items.Count(t => t["create"]["ok"].Value<bool>().Equals(true));
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
