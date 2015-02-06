using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam
{
	public class ToFromRangeAggregationParameter : RangeAggregationParameter
	{
		private readonly object _to;
		private readonly object _from;

		public ToFromRangeAggregationParameter(object to, object from)
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