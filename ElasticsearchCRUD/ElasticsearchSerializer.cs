using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	public class ElasticsearchSerializer  : IDisposable
	{
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver;
		private readonly ITraceProvider _traceProvider;

		public ElasticsearchSerializer( ITraceProvider traceProvider, IElasticSearchMappingResolver elasticSearchMappingResolver)
		{
			_elasticSearchMappingResolver = elasticSearchMappingResolver;
			_traceProvider = traceProvider;
		}

		private JsonWriter _writer;

		public string Serialize(IEnumerable<Tuple<EntityContextInfo, object>> entities)
		{
			if (entities == null)
			{
				return null;
			}

			var sb = new StringBuilder();
			_writer = new JsonTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture)) { CloseOutput = true };

			foreach (var entity in entities)
			{
				string index = _elasticSearchMappingResolver.GetElasticSearchMapping(entity.GetType()).GetIndexForType(entity.GetType());
				if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
				{
					_traceProvider.Trace(TraceEventType.Error, "index is not allowed in Elasticsearch: {0}", index);
					throw new ElasticsearchCrudException(string.Format("index is not allowed in Elasticsearch: {0}", index));
				}
				if (entity.Item1.DeleteEntity)
				{
					DeleteEntity(entity.Item1);
				}
				else
				{
					AddUpdateEntity(entity.Item2, entity.Item1);
				}
			}

			_writer.Close();
			_writer = null;

			return sb.ToString();
		}

		private void DeleteEntity(EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticSearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			_writer.WriteStartObject();

			_writer.WritePropertyName("delete");

			// Write the batch "index" operation header
			_writer.WriteStartObject();
			WriteValue("_index", elasticSearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticSearchMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);
			_writer.WriteEndObject();
			_writer.WriteEndObject();
		
			_writer.WriteRaw("\n");
		}

		private void AddUpdateEntity(object entity, EntityContextInfo entityInfo)
		{
			var elasticSearchMapping = _elasticSearchMappingResolver.GetElasticSearchMapping(entityInfo.EntityType);
			_writer.WriteStartObject();

			_writer.WritePropertyName("index");

			// Write the batch "index" operation header
			_writer.WriteStartObject();
			WriteValue("_index", elasticSearchMapping.GetIndexForType(entityInfo.EntityType));
			WriteValue("_type", elasticSearchMapping.GetDocumentType(entityInfo.EntityType));
			WriteValue("_id", entityInfo.Id);
			_writer.WriteEndObject();
			_writer.WriteEndObject();
			_writer.WriteRaw("\n");  //ES requires this \n separator

			_writer.WriteStartObject();

			elasticSearchMapping.MapEntityValues(entity, _writer, true);

			_writer.WriteEndObject();
			_writer.WriteRaw("\n");
		}

		private void WriteValue(string key, object valueObj)
		{
			_writer.WritePropertyName(key);
			_writer.WriteValue(valueObj);
		}

		public void Dispose()
		{
			if (_writer != null)
			{
				_writer.Close();
				_writer = null;
			}
		}
	}
}
