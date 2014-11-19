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
		private readonly bool _saveChangesAndInitMappingsForChildDocuments;
		private ElasticSerializationResult _elasticSerializationResult = new ElasticSerializationResult();
		private readonly IndexMappings _indexMappings;

		public ElasticsearchSerializer(ITraceProvider traceProvider, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, bool saveChangesAndInitMappingsForChildDocuments)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_saveChangesAndInitMappingsForChildDocuments = saveChangesAndInitMappingsForChildDocuments;
			_traceProvider = traceProvider;
			_indexMappings = new IndexMappings(_traceProvider, _elasticsearchSerializerConfiguration);
			_elasticSerializationResult.IndexMappings = _indexMappings;
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
			_elasticSerializationResult.IndexMappings = _indexMappings;
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

			if (_saveChangesAndInitMappingsForChildDocuments)
			{
				_indexMappings.CreatePropertyMappingForTopEntity(entityInfo);
			}

			CreateBulkContentForParentDocument(entityInfo, elasticSearchMapping);

			if (_elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			{
				if (elasticSearchMapping.ChildIndexEntities.Count > 1)
				{
					// Only save the top level items now
					elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
					foreach (var item in elasticSearchMapping.ChildIndexEntities)
					{
						CreateBulkContentForChildDocument(entityInfo, elasticSearchMapping, item);
					}
				}
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
			if (entityInfo.RoutingDefinition.RoutingId != null && _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex &&
				_elasticsearchSerializerConfiguration.UserDefinedRouting)
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
			if (item.RoutingDefinition.RoutingId != null && _elasticsearchSerializerConfiguration.UserDefinedRouting)
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
