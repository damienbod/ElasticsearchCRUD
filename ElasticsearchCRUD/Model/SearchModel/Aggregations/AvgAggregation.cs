namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class AvgAggregation : BaseMetricAggregation
	{
		public AvgAggregation(string name, string field) : base("avg", name, field) { }
	}
}