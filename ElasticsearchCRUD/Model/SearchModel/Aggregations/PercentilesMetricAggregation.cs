namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class PercentilesMetricAggregation : BaseMetricAggregation
	{
		public PercentilesMetricAggregation(string name, string field) : base("percentiles", name, field) { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
		}
	}
}