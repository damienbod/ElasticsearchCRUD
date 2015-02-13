using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

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
				string index = _elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(entity.EntityType).GetIndexForType(entity.EntityType);
				MappingUtils.GuardAgainstBadIndexName(index);

				if (_saveChangesAndInitMappingsForChildDocuments)
				{
					_indexMappings.CreateIndexSettingsForDocument(index, new IndexSettings{NumberOfShards=5,NumberOfReplicas=1}, new IndexAliases(), new IndexWarmers() );
					_indexMappings.CreatePropertyMappingForTopDocument(entity, new MappingDefinition{Index=index});
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
				if (elasticSearchMapping.ChildIndexEntities.Count > 0)
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
			if (entityInfo.RoutingDefinition.RoutingId != null &&
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
			var childMapping =
				_elasticsearchSerializerConfiguration.ElasticsearchMappingResolver.GetElasticSearchMapping(item.EntityType);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
			// Write the batch "index" operation header
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			// Always write to the same index
			WriteValue("_index", childMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", childMapping.GetDocumentType(item.EntityType));
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

			childMapping.MapEntityValues(item, _elasticsearchCrudJsonWriter, true);

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
