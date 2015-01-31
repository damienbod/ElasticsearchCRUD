namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class SumAggregation : BaseAggregation
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

