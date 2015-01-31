namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class SumAggregation : BaseMetricAggregation
	{
		public SumAggregation(string name, string field) : base("sum", name, field) { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
		}
	}
}

