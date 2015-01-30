namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class SumAggregation : BaseMetricAggregation
	{
		public SumAggregation(string name, string field) : base("sum", name, field) { }
	}
}

