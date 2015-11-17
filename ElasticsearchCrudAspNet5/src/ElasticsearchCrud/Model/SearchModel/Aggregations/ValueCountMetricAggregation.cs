namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class ValueCountMetricAggregation : BaseMetricAggregation
	{
		public ValueCountMetricAggregation(string name, string field) : base("value_count", name, field) { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
		}
	}
}