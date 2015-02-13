using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Model;
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
				traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT content: {0}", command.Content, "InitMappings");
				
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

		public void CreateIndexSettingsForDocument(string index, IndexSettings indexSettings, IndexAliases indexAliases, IndexWarmers indexWarmers)
		{
			if (_processedItems.Contains("_index" + index))
			{
				return;
			}
			_processedItems.Add("_index" + index);
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			CreateIndexSettings(elasticsearchCrudJsonWriter, indexSettings);
			indexAliases.WriteJson(elasticsearchCrudJsonWriter);
			indexWarmers.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			CreateIndexCommand(elasticsearchCrudJsonWriter.GetJsonString(), index);
		}

		public void UpdateSettings(string index, IndexUpdateSettings indexUpdateSettings)
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			indexUpdateSettings.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			SettingsCommand(elasticsearchCrudJsonWriter.GetJsonString(), index);
		}
		public void CreatePropertyMappingForTopDocument(EntityContextInfo entityInfo, MappingDefinition mappingDefinition)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticsearchSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;

			CreatePropertyMappingForEntityForParentDocument(entityInfo, elasticSearchMapping, mappingDefinition);

			if (_elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				if (elasticSearchMapping.ChildIndexEntities.Count > 0)
				{
					// Only save the top level items now
					elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
					foreach (var item in elasticSearchMapping.ChildIndexEntities)
					{
						CreatePropertyMappingForChildDocument(entityInfo, elasticSearchMapping, item, mappingDefinition);
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
		/// <param name="mappingDefinition">mapping definitions for the index type</param>
		private void CreatePropertyMappingForEntityForParentDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping, MappingDefinition mappingDefinition)
		{
			var itemType = elasticsearchMapping.GetDocumentType(entityInfo.EntityType);
			if (_processedItems.Contains("_mapping" +itemType))
			{
				return;
			}
			_processedItems.Add("_mapping" +itemType);

			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(itemType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			mappingDefinition.Source.WriteJson(elasticsearchCrudJsonWriter);
			mappingDefinition.All.WriteJson(elasticsearchCrudJsonWriter);
			mappingDefinition.Analyzer.WriteJson(elasticsearchCrudJsonWriter);

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

			CreateMappingCommandForTypeWithExistingIndex(elasticsearchCrudJsonWriter.GetJsonString(), mappingDefinition.Index, itemType);
		
		}

		/// <summary>
		/// Create a new mapping for the child type in the parent document index
		/// </summary>
		/// <param name="entityInfo"></param>
		/// <param name="elasticsearchMapping"></param>
		/// <param name="item"></param>
		/// <param name="mappingDefinition">definition for the type mappings</param>
		private void CreatePropertyMappingForChildDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping, EntityContextInfo item, MappingDefinition mappingDefinition)
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

			mappingDefinition.Source.WriteJson(elasticsearchCrudJsonWriter);
			mappingDefinition.All.WriteJson(elasticsearchCrudJsonWriter);
			mappingDefinition.Analyzer.WriteJson(elasticsearchCrudJsonWriter);

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
			//Console.WriteLine("XXXCreateMappingCommandForTypeWithExistingIndex: " + index + ": " + documentType);
			Commands.Add(command);
		}

		private void SettingsCommand(string indexJsonConfiguration, string index)
		{
			var command = new MappingCommand
			{
				Url = string.Format("/{0}/_settings", index),
				RequestType = "PUT",
				Content = indexJsonConfiguration
			};
			//Console.WriteLine("XXXSettingsCommand: " + index);
			Commands.Add(command);
		}

		private void CreateIndexCommand(string indexJsonConfiguration, string index)
		{
			var command = new MappingCommand
			{
				Url = string.Format("/{0}", index),
				RequestType = "PUT",
				Content = indexJsonConfiguration
			};
			//Console.WriteLine("XXXCreateIndexCommand: " + index);
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
		private void CreateIndexSettings(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, IndexSettings indexSettings)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("settings");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			indexSettings.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}


}
