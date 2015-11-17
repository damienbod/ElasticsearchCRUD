
using System.Collections.Generic;

namespace ElasticsearchCRUD.Utils
{
	public static class JsonHelper
	{
		public static void WriteValue(string key, object valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
			}
		}

		public static void WriteListValue(string key, List<string> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var obj in valueObj)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}

		public static void WriteListValue(string key, List<double> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var obj in valueObj)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}

		public static void WriteListValue(string key, List<object> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var obj in valueObj)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}
	}
}
