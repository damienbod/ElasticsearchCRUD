namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class ExtendedStatsAggregation : BaseMetricAggregation
	{
		public ExtendedStatsAggregation(string name, string field) : base("extended_stats", name, field) { }
	}
}