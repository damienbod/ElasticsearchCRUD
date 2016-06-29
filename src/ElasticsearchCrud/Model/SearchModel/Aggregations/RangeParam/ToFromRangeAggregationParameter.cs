using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam
{
	public class ToFromRangeAggregationParameter<T> : RangeAggregationParameter<T>
	{
		private readonly T _to;
		private readonly T _from;

		public ToFromRangeAggregationParameter(T to, T from)
		{
			_to = to;
			_from = @from;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("key", KeyValue, elasticsearchCrudJsonWriter, KeySet);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("to");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_to);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("from");
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_from);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}