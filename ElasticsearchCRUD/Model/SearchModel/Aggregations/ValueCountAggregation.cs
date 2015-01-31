namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class ValueCountAggregation : BaseMetricAggregation
	{
		public ValueCountAggregation(string name, string field) : base("value_count", name, field) { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
		}
	}
}