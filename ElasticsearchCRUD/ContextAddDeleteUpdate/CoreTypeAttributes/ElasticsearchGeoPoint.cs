using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ElasticsearchGeoPoint : ElasticsearchCoreTypes
	{
		public override string JsonString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", "geo_point", elasticsearchCrudJsonWriter);
			//JsonHelper.WriteValue("index", _index.ToString(), elasticsearchCrudJsonWriter, _indexSet);
			//JsonHelper.WriteValue("index_name", _indexName, elasticsearchCrudJsonWriter, _indexNameSet);
			//JsonHelper.WriteValue("store", _store, elasticsearchCrudJsonWriter, _storeSet);
			//JsonHelper.WriteValue("doc_values", _docValues, elasticsearchCrudJsonWriter, _docValuesSet);
			//JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			//JsonHelper.WriteValue("null_value", _nullValue, elasticsearchCrudJsonWriter, _nullValueSet);

			WriteBaseValues(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			return elasticsearchCrudJsonWriter.Stringbuilder.ToString();
		}
	}
}