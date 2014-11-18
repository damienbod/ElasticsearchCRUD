
using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
	public class ElasticsearchFloat : ElasticsearchNumber
	{
		public override string JsonString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteValue("type", "float", elasticsearchCrudJsonWriter);
			WriteValue("index_name", _indexName, elasticsearchCrudJsonWriter, _indexNameSet);
			WriteValue("store", _store, elasticsearchCrudJsonWriter, _storeSet);
			WriteValue("index", _index.ToString(), elasticsearchCrudJsonWriter, _indexSet);
			WriteValue("doc_values", _docValues, elasticsearchCrudJsonWriter, _docValuesSet);
			WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			WriteValue("null_value", _nullValue, elasticsearchCrudJsonWriter, _nullValueSet);
			WriteValue("include_in_all", _includeInAll, elasticsearchCrudJsonWriter, _includeInAllSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			return elasticsearchCrudJsonWriter.Stringbuilder.ToString();
		}
	}
}
