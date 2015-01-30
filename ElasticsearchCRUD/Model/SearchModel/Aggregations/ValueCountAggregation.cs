namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class ValueCountAggregation : BaseMetricAggregation
	{
		public ValueCountAggregation(string name, string field) : base("value_count", name, field) { }
	}
}