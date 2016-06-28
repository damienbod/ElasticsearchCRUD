namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class FilterBucketAggregation : BaseBucketAggregation
	{
		private readonly IFilter _filter;

		public FilterBucketAggregation(string name, IFilter filter) : base("filter", name)
		{
			_filter = filter;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			_filter.WriteJson(elasticsearchCrudJsonWriter);
		}
	}
}