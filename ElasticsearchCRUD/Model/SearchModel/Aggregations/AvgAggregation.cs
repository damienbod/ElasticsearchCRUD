namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class AvgAggregation : BaseAggregation
	{
		public AvgAggregation(string name, string field) : base("avg", name, field) { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
		}
	}
}