using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	public class ElasticsearchSerializer<T>  : IDisposable where T : class
	{
		private readonly ElasticSearchSerializerMapping<T> _elasticSearchSerializerMapping;
		private readonly ITraceProvider _traceProvider;

		public ElasticsearchSerializer(ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping, ITraceProvider traceProvider)
		{
			_elasticSearchSerializerMapping = elasticSearchSerializerMapping;
			_traceProvider = traceProvider;
		}

		private JsonWriter _writer;

		public string Serialize(IEnumerable<Tuple<EntityContextInfo, T>> entities)
		{
			if (entities == null)
			{
				return null;
			}

			var sb = new StringBuilder();
			_writer = new JsonTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture)) { CloseOutput = true };

			foreach (var entity in entities)
			{
				if (Regex.IsMatch(entity.Item1.Index, "[\\\\/*?\",<>|\\sA-Z]"))
				{
					_traceProvider.Trace(string.Format("index is not allowed in Elasticsearch: {0}", entity.Item1.Index));
					throw new ArgumentException(string.Format("index is not allowed in Elasticsearch: {0}", entity.Item1.Index));
				}
				if (entity.Item1.DeleteEntity)
				{
					DeleteEntity(entity.Item1);
				}
				else
				{
					AddUpdateEntity(entity.Item2, entity.Item1, _elasticSearchSerializerMapping);
				}
			}

			_writer.Close();
			_writer = null;

			return sb.ToString();
		}

		private void DeleteEntity(EntityContextInfo entityInfo)
		{
			_writer.WriteStartObject();

			_writer.WritePropertyName("delete");

			// Write the batch "index" operation header
			_writer.WriteStartObject();
			WriteValue("_index", entityInfo.Index);
			WriteValue("_type", _elasticSearchSerializerMapping.GetDocumentType(typeof(T)));
			WriteValue("_id", entityInfo.Id);
			_writer.WriteEndObject();
			_writer.WriteEndObject();
		
			_writer.WriteRaw("\n");
		}

		private void AddUpdateEntity(T entity, EntityContextInfo entityInfo, ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping)
		{
			_writer.WriteStartObject();

			_writer.WritePropertyName("index");

			// Write the batch "index" operation header
			_writer.WriteStartObject();
			WriteValue("_index", entityInfo.Index);
			WriteValue("_type", _elasticSearchSerializerMapping.GetDocumentType(typeof(T)));
			WriteValue("_id", entityInfo.Id);
			_writer.WriteEndObject();
			_writer.WriteEndObject();
			_writer.WriteRaw("\n");  //ES requires this \n separator

			_writer.WriteStartObject();

			elasticSearchSerializerMapping.AddWriter(_writer);
			elasticSearchSerializerMapping.MapEntityValues(entity);

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
