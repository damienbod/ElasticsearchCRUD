using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate
{
	public class IndexMappings
	{
		private readonly ITraceProvider _traceProvider;
		private ElasticsearchCrudJsonWriter _elasticsearchCrudJsonWriter;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private ElasticSerializationResult _elasticSerializationResult = new ElasticSerializationResult();
		private bool createPropertyMappings = true;

		public List<string> CommandTypes = new List<string>();
		public List<MappingCommand> Commands = new List<MappingCommand>();

		public async Task<ResultDetails<string>> Execute(HttpClient client, string baseUrl, ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource)
		{
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };
			foreach (var command in Commands)
			{
				var content = new StringContent(command.Content + "\n");
				traceProvider.Trace(TraceEventType.Verbose, "{1}: sending init mappings request: {0}", command, "InitMappings");
				traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT uri: {0}", command.Url, "InitMappings");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				HttpResponseMessage response;
				if (command.RequestType == "POST")
				{
					response = await client.PostAsync(baseUrl + command.Url, content, cancellationTokenSource.Token).ConfigureAwait(true);
				}
				else
				{
					response = await client.PutAsync(baseUrl + command.Url, content, cancellationTokenSource.Token).ConfigureAwait(true);
				}

				//resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
				{
					traceProvider.Trace(TraceEventType.Warning, "{2}: SaveChangesAsync response status code: {0}, {1}",
						response.StatusCode, response.ReasonPhrase, "InitMappings");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}

					return resultDetails;
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
				traceProvider.Trace(TraceEventType.Verbose, "{1}: response: {0}", responseString, "InitMappings");
			}

			// no errors
			resultDetails.Status = HttpStatusCode.OK;
			return resultDetails;
		}


		public IndexMappings(ITraceProvider traceProvider, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_traceProvider = traceProvider;
		}

		public void CreatePropertyMappingForTopEntity(EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticsearchSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;

			CreatePropertyMappingForEntityForParentDocument(entityInfo, elasticSearchMapping);

			if (_elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				if (elasticSearchMapping.ChildIndexEntities.Count > 1)
				{
					// Only save the top level items now
					elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
					foreach (var item in elasticSearchMapping.ChildIndexEntities)
					{
						CreatePropertyMappingForChildDocument(entityInfo, elasticSearchMapping, item);
					}
				}
			}
			elasticSearchMapping.ChildIndexEntities.Clear();
		}

		private readonly List<string> _processedItems = new List<string>(); 
		private void CreatePropertyMappingForChildDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping, EntityContextInfo item)
		{
			ProccessPropertyMappings(item, elasticsearchMapping);
		}

		private void CreatePropertyMappingForEntityForParentDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping)
		{
			ProccessPropertyMappings(entityInfo, elasticsearchMapping);
		}

		private void ProccessPropertyMappings(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping)
		{
			var itemType = elasticsearchMapping.GetDocumentType(entityInfo.EntityType);
			if (_processedItems.Contains(itemType))
			{
				return;
			}
			_processedItems.Add(itemType);

			_elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			//"skill": {
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(elasticsearchMapping.GetDocumentType(entityInfo.EntityType));
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			//"properties": {
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("properties");
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchMapping.MapEntityValues(entityInfo, _elasticsearchCrudJsonWriter, true, createPropertyMappings);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");

			// TODO add to command
		}
	}


}
