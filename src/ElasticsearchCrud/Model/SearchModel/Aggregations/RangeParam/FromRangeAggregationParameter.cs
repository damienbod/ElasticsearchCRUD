using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam
{
	public class FromRangeAggregationParameter<T> : RangeAggregationParameter<T>
	{
		private readonly T _value;

		public FromRangeAggregationParameter(T value)
		{
			_value = value;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("key", KeyValue, elasticsearchCrudJsonWriter, KeySet);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("from");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_value);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}