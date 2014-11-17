using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate
{
	public class IndexMapping
	{
		private readonly ITraceProvider _traceProvider;
		private ElasticsearchCrudJsonWriter _elasticsearchCrudJsonWriter;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private ElasticSerializationResult _elasticSerializationResult = new ElasticSerializationResult();
		private bool createPropertyMappings = true;
		public IndexMapping(ITraceProvider traceProvider, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration)
		{
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_traceProvider = traceProvider;
		}


		private string CreateParentMapping(string parentType, string childType)
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(childType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_parent");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("type");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(parentType);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			return elasticsearchCrudJsonWriter.Stringbuilder.ToString();
		}

		public void CreatePropertyMappingForEntity(EntityContextInfo entityInfo)
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

		private void CreatePropertyMappingForChildDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping, EntityContextInfo item)
		{
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchMapping.MapEntityValues(entityInfo, _elasticsearchCrudJsonWriter, true, createPropertyMappings);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void CreatePropertyMappingForEntityForParentDocument(EntityContextInfo entityInfo, ElasticsearchMapping elasticsearchMapping)
		{
			//_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			//_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
			//// Write the batch "index" operation header
			//_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			//WriteValue("_index", elasticsearchMapping.GetIndexForType(entityInfo.EntityType));
			//WriteValue("_type", elasticsearchMapping.GetDocumentType(entityInfo.EntityType));
			//WriteValue("_id", entityInfo.Id);
			//if (entityInfo.RoutingDefinition.ParentId != null && _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex)
			//{
			//	// It's a document which belongs to a parent
			//	WriteValue("_parent", entityInfo.RoutingDefinition.ParentId);
			//}
			//if (entityInfo.RoutingDefinition.RoutingId != null && _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex &&
			//	_elasticsearchSerializerConfiguration.UserDefinedRouting)
			//{
			//	// It's a document which has a specific route
			//	WriteValue("_routing", entityInfo.RoutingDefinition.RoutingId);
			//}
			//_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			//_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			//_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n"); //ES requires this \n separator
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchMapping.MapEntityValues(entityInfo, _elasticsearchCrudJsonWriter, true, createPropertyMappings);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");
		}

		private void WriteValue(string key, object valueObj)
		{
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
			_elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
		}
	}


}
