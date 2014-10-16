using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ElasticsearchCRUD.Mapping;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD
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
				string index = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(entity.GetType()).GetIndexForType(entity.GetType());
				if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
				{
					_traceProvider.Trace(TraceEventType.Error, "{1}: index is not allowed in Elasticsearch: {0}", index, "ElasticsearchCrudJsonWriter");
					throw new ElasticsearchCrudException(string.Format("ElasticsearchCrudJsonWriter: index is not allowed in Elasticsearch: {0}", index));
				}
				if (entity.DeleteEntity)
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
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
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
			var elasticSearchMapping = _elasticsearchSerializerConfiguration.ElasticSearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			elasticSearchMapping.TraceProvider = _traceProvider;
			elasticSearchMapping.SaveChildObjectsAsWellAsParent = _elasticsearchSerializerConfiguration.SaveChildObjectsAsWellAsParent;
			elasticSearchMapping.ProcessChildDocumentsAsSeparateChildIndex = _elasticsearchSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex;
			
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
			// Write the batch "index" operation header
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			WriteValue("_index", elasticSearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticSearchMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");  //ES requires this \n separator
			_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticSearchMapping.MapEntityValues(entityInfo, _elasticsearchCrudJsonWriter, true);

			_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");

			var initMappings = new InitMappings();
			initMappings.CreateIndex(
						elasticSearchMapping.GetIndexForType(entityInfo.EntityType),
						elasticSearchMapping.GetDocumentType(entityInfo.EntityType),
						null,
						entityInfo.Id, "{dddd}");

			if (elasticSearchMapping.ChildIndexEntities.Count > 1)
			{
				// Only save the top level items now
				elasticSearchMapping.SaveChildObjectsAsWellAsParent = false;
				foreach (var item in elasticSearchMapping.ChildIndexEntities)
				{
					_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					_elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("index");
					// Write the batch "index" operation header
					_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					
					// Always write to the same index
					WriteValue("_index", elasticSearchMapping.GetIndexForType(entityInfo.EntityType));
					WriteValue("_type", elasticSearchMapping.GetDocumentType(item.EntityType));
					WriteValue("_id", item.Id);
					WriteValue("_parent", item.ParentId);
					_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
					_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
					_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");  //ES requires this \n separator
					_elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

					elasticSearchMapping.MapEntityValues(item, _elasticsearchCrudJsonWriter, true);

					_elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
					_elasticsearchCrudJsonWriter.JsonWriter.WriteRaw("\n");

					initMappings.CreateIndexMapping(
						elasticSearchMapping.GetIndexForType(entityInfo.EntityType),
						elasticSearchMapping.GetDocumentType(item.ParentEntityType),
						elasticSearchMapping.GetDocumentType(item.Entity.GetType())
					);

					initMappings.CreateIndex(
						elasticSearchMapping.GetIndexForType(entityInfo.EntityType),
						elasticSearchMapping.GetDocumentType(item.EntityType),
						item.ParentId.ToString(),
						item.Id, "{dddd}");
				}
			}

			_elasticSerializationResult.CommandsForAllEntities.AddRange(initMappings.Commands);
			elasticSearchMapping.ChildIndexEntities.Clear();			
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
