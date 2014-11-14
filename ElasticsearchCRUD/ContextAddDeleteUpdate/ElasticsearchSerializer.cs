using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate
{
	public class ElasticsearchSerializer  : IDisposable
	{
		private readonly ITraceProvider _traceProvider;
		private ElasticsearchCrudJsonWriter _elasticsearchCrudJsonWriter;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private ElasticSerializationResult _elasticSerializationResult = new ElasticSerializationResult();

		public ElasticsearchSerializer(ITraceProvider traceProvider, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_traceProvider = traceProvider;
		}

		public ElasticSerializationResult Serialize(IEnumerable<EntityContextInfo> entities)
		{
			if (entities == null)
			{
				return null;
			}
			_elasticSerializationResult = new ElasticSerializationResult();
			_elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();

			foreach (var entity in entities)
			{
				string index = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entity.GetType()).GetIndexForType(entity.GetType());
				if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
				{
					_traceProvider.Trace(TraceEventType.Error, "{1}: index is not allowed in Elasticsearch: {0}", index, "ElasticsearchCrudJsonWriter");
					throw new ElasticsearchCrudException(string.Format("ElasticsearchCrudJsonWriter: index is not allowed in Elasticsearch: {0}", index));
				}
				if (entity.DeleteDocument)
				{
					DeleteEntity(entity);
				}
				else
				{
					AddUpdateEntity(entity);
				}
			}

			_elasticsearchCrudJsonWriter.Dispose();
			_elasticSerializationResult.Content = _elasticsearchCrudJsonWriter.Stringbuilder.ToString();
			return _elasticSerializationResult;
		}

		private void DeleteEntity(EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticsearchSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("delete");

			// Write the batch "index" operation header
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			WriteValue("_index", elasticSearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticSearchMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();	
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void AddUpdateEntity(EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticsearchSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;
			
			CreateBulkContentForParentDocument(entityInfo, elasticSearchMapping);

			if (_elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				var initMappings = InitMappingIndex(entityInfo, elasticSearchMapping);

				if (elasticSearchMapping.ChildIndexEntities.Count > 1)
				{
					// Only save the top level items now
					elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
					foreach (var item in elasticSearchMapping.ChildIndexEntities)
					{
						CreateBulkContentForChildDocument(entityInfo, elasticSearchMapping, item);
						CreateCommandForChildDocument(entityInfo, initMappings, elasticSearchMapping, item);
					}
				}

				_elasticSerializationResult.InitMappings = initMappings;
			}
			elasticSearchMapping.ChildIndexEntities.Clear();			
		}

		private void CreateBulkContentForParentDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping)
		{
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
			// Write the batch "index" operation header
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			WriteValue("_index", elasticsearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticsearchMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);		
			if (entityInfo.RoutingDefinition.ParentId != null && _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				// It's a document which belongs to a parent
				WriteValue("_parent", entityInfo.RoutingDefinition.ParentId);
			}
			if (entityInfo.RoutingDefinition.RoutingId != null && _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				// It's a document which has a specific route
				WriteValue("_routing", entityInfo.RoutingDefinition.RoutingId);
			}
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n"); //ES requires this \n separator
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchMapping.MapEntityValues(entityInfo, _elasticsearchCrudJsonWriter, true);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void CreateBulkContentForChildDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping,
			EntityContextInfo item)
		{
			// TODO Delete document if exists
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
			// Write the batch "index" operation header
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			// Always write to the same index
			WriteValue("_index", elasticsearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticsearchMapping.GetDocumentType(item.EntityType));
			WriteValue("_id", item.Id);
			WriteValue("_parent", item.RoutingDefinition.ParentId);
			if (item.RoutingDefinition.RoutingId != null)
			{
				// It's a document which has a specific route
				WriteValue("_routing", item.RoutingDefinition.RoutingId);
			}
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n"); //ES requires this \n separator
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchMapping.MapEntityValues(item, _elasticsearchCrudJsonWriter, true);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private static void CreateCommandForChildDocument(EntityContextInfo entityInfo, InitMappings initMappings,
			ElasticsearchMapping elasticsearchMapping, EntityContextInfo item)
		{
			initMappings.CreateIndexMapping(
				elasticsearchMapping.GetIndexForType(entityInfo.EntityType),
				elasticsearchMapping.GetDocumentType(item.ParentEntityType),
				elasticsearchMapping.GetDocumentType(item.Document.GetType())
				);

			var contentJson = new ElasticsearchCrudJsonWriter();
			contentJson.JsonWriter.WriteStartObject();
			elasticsearchMapping.MapEntityValues(item, contentJson, true);
			contentJson.JsonWriter.WriteEndObject();

			initMappings.CreateIndex(
				elasticsearchMapping.GetIndexForType(entityInfo.EntityType),
				elasticsearchMapping.GetDocumentType(item.EntityType),
				item.RoutingDefinition.ParentId.ToString(),
				item.Id, contentJson.GetJsonString());
		}

		private static InitMappings InitMappingIndex(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping)
		{
			var contentJsonParent = new ElasticsearchCrudJsonWriter();
			contentJsonParent.JsonWriter.WriteStartObject();
			elasticsearchMapping.MapEntityValues(entityInfo, contentJsonParent, true);
			contentJsonParent.JsonWriter.WriteEndObject();
			var initMappings = new InitMappings();

			initMappings.CreateIndex(
				elasticsearchMapping.GetIndexForType(entityInfo.EntityType),
				elasticsearchMapping.GetDocumentType(entityInfo.EntityType),
				null,
				entityInfo.Id, contentJsonParent.GetJsonString());
			return initMappings;
		}

		private void WriteValue(string key, object valueObj)
		{
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
			_elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
		}

		public void Dispose()
		{
			_elasticsearchCrudJsonWriter.Dispose();
		}
	}
}
