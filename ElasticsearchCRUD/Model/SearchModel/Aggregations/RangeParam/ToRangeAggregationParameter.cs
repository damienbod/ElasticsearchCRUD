using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam
{
	public class ToRangeAggregationParameter<T> : RangeAggregationParameter<T>
	{
		private readonly T _value;

		public ToRangeAggregationParameter(T value)
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