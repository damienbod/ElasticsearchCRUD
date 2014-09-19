using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Damienbod.ElasticSearchProvider
{
	public class ElasticsearchSerializer<T>  : IDisposable where T : class
	{
		private readonly ElasticSearchSerializerMapping<T> _elasticSearchSerializerMapping;

		public ElasticsearchSerializer(ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping)
		{
			_elasticSearchSerializerMapping = elasticSearchSerializerMapping;
		}

		private JsonWriter _writer;

		public string Serialize(IEnumerable<T> entities, string index)
		{
			if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				throw new ArgumentException(string.Format("index is not allowed in Elasticsearch: {0}", index));
			}

			if (entities == null)
			{
				return null;
			}

			var sb = new StringBuilder();
			_writer = new JsonTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture)) { CloseOutput = true };

			foreach (var entry in entities)
			{
				WriteJsonEntry(entry, index, _elasticSearchSerializerMapping);
			}

			_writer.Close();
			_writer = null;

			return sb.ToString();
		}

		private void WriteJsonEntry(T entity, string index, ElasticSearchSerializerMapping<T> elasticSearchSerializerMapping)
		{
			_writer.WriteStartObject();

			_writer.WritePropertyName("index");

			// Write the batch "index" operation header
			_writer.WriteStartObject();
			WriteValue("_index", index);
			WriteValue("_type", typeof(T).ToString());
			_writer.WriteEndObject();
			_writer.WriteEndObject();
			_writer.WriteRaw("\n");  //ES requires this \n separator

			_writer.WriteStartObject();

			elasticSearchSerializerMapping.AddWriter(_writer);
			elasticSearchSerializerMapping.WriteJsonEntry(entity);

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
