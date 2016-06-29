namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class GlobalBucketAggregation : BaseBucketAggregation
	{
		private readonly string _name;

		public GlobalBucketAggregation(string name) : base("global", name)
		{
			_name = name;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			
		}
	}
}