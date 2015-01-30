namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class MaxAggregation : BaseMetricAggregation
	{
		public MaxAggregation(string name, string field) : base("max", name, field) { }
	}
}