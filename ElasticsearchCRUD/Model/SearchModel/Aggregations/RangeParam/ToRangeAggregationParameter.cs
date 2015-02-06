using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam
{
	public class ToRangeAggregationParameter : RangeAggregationParameter
	{
		private readonly object _value;

		public ToRangeAggregationParameter(object value)
		{
			_value = value;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("key", KeyValue, elasticsearchCrudJsonWriter, KeySet);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("to");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_value);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}