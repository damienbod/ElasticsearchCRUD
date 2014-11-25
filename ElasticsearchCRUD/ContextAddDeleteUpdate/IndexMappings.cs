using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate
{
	public class IndexMappings
	{
		private readonly ITraceProvider _traceProvider;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private const bool CreatePropertyMappings = true;
		private readonly List<string> _processedItems = new List<string>();

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

				if (response.StatusCode == HttpStatusCode.BadRequest )
				{
					var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
					var responseObject = JObject.Parse(errorInfo);
					var source = responseObject["error"];
					throw new ElasticsearchCrudException("IndexMappings: Execute Request POST BadRequest: " + source);
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

		public void CreateIndexSettingsForDocument(string index, IndexSettings indexSettings)
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();

			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			CreateIndexSettings(elasticsearchCrudJsonWriter, indexSettings.number_of_shards, indexSettings.number_of_replicas);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			CreateIndexCommand(elasticsearchCrudJsonWriter.GetJsonString(), index);
		}

		public void CreatePropertyMappingForTopDocument(EntityContextInfo entityInfo, string index)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticsearchSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;

			CreatePropertyMappingForEntityForParentDocument(entityInfo, elasticSearchMapping, index);

			if (_elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				if (elasticSearchMapping.ChildIndexEntities.Count > 0)
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

		/// <summary>
		/// Create a new index for the parent document
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <param name="elasticsearchMapping"></param>
		/// <param name="index"></param>
		private void CreatePropertyMappingForEntityForParentDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping, string index)
		{
			var itemType = elasticsearchMapping.GetDocumentType(entityInfo.EntityType);
			if (_processedItems.Contains(itemType))
			{
				return;
			}
			_processedItems.Add(itemType);

			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(itemType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (entityInfo.RoutingDefinition.RoutingId != null && _elasticsearchSerializerConfiguration.UserDefinedRouting)
			{
				CreateForceRoutingMappingForDocument(elasticsearchCrudJsonWriter);
			}

			if (entityInfo.RoutingDefinition.ParentId != null)
			{
				CreateParentMappingForDocument(
				elasticsearchCrudJsonWriter,
				elasticsearchMapping.GetDocumentType(entityInfo.ParentEntityType));
			}

			ProccessPropertyMappingsWithoutTypeName(elasticsearchCrudJsonWriter, entityInfo, elasticsearchMapping);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			CreateMappingCommandForTypeWithExistingIndex(elasticsearchCrudJsonWriter.GetJsonString(), index, itemType);
		
		}

		/// <summary>
		/// Create a new mapping for the child type in the parent document index
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <param name="elasticsearchMapping"></param>
		/// <param name="item"></param>
		private void CreatePropertyMappingForChildDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping, EntityContextInfo item)
		{
			var childMapping =
				_elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(item.EntityType);

			var parentMapping =
				_elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(item.ParentEntityType);

			var childType = childMapping.GetDocumentType(item.EntityType);
			var parentType = parentMapping.GetDocumentType(item.ParentEntityType);

			var processedId = childType + "_" + parentType;
			if (_processedItems.Contains(childType))
			{
				var test = CommandTypes.Find(t => t.StartsWith(childType));
				if (test != processedId)
				{
					throw new ElasticsearchCrudException("InitMappings: Not supported, child documents can only have one parent");
				}
				return;
			}
			_processedItems.Add(childType);
			CommandTypes.Add(processedId);

			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(childType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			CreateParentMappingForDocument(
				elasticsearchCrudJsonWriter,
				elasticsearchMapping.GetDocumentType(item.ParentEntityType));

			if (item.RoutingDefinition.RoutingId != null && _elasticsearchSerializerConfiguration.UserDefinedRouting)
			{
				CreateForceRoutingMappingForDocument(elasticsearchCrudJsonWriter);
			}

			ProccessPropertyMappingsWithoutTypeName(elasticsearchCrudJsonWriter, item, childMapping);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			CreateMappingCommandForTypeWithExistingIndex(elasticsearchCrudJsonWriter.GetJsonString(), elasticsearchMapping.GetIndexForType(entityInfo.EntityType), childMapping.GetDocumentType(item.EntityType));
		}

		private void ProccessPropertyMappingsWithoutTypeName(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping)
		{
			//"properties": {
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("properties");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchMapping.MapEntityValues(entityInfo, elasticsearchCrudJsonWriter, true, CreatePropertyMappings);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void CreateMappingCommandForTypeWithExistingIndex(string propertyMapping, string index, string documentType)
		{
			var command = new MappingCommand
			{
				Url = string.Format("/{0}/{1}/_mappings", index, documentType),
				RequestType = "PUT",
				Content = propertyMapping
			};
			Commands.Add(command);
		}

		private void CreateIndexCommand(string indexJsonConfiguration, string index)
		{
			var command = new MappingCommand
			{
				Url = string.Format("/{0}/", index),
				RequestType = "PUT",
				Content = indexJsonConfiguration
			};
			Commands.Add(command);
		}

		private void CreateParentMappingForDocument(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, string parentType)
		{	
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_parent");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("type");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(parentType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		/// <summary>
		/// "_routing": {
		/// "required": true
		/// },
		/// </summary>
		/// <param name="elasticsearchCrudJsonWriter"></param>
		private void CreateForceRoutingMappingForDocument(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_routing");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("required");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue("true");
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		/// "settings" : {
		/// "number_of_shards" : 1
		/// },
		private void CreateIndexSettings(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, int numberOfShards, int numberOfReplicas)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("settings");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("number_of_shards");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(numberOfShards);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("number_of_replicas");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(numberOfReplicas);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}


}
