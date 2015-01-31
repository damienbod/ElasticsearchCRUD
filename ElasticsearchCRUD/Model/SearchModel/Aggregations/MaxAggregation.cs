namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class MaxAggregation : BaseMetricAggregation
	{
		public MaxAggregation(string name, string field) : base("max", name, field) { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
		}
	}
}