using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A special single bucket aggregation that enables aggregating nested documents.
	/// </summary>
	public class NestedBucketAggregation : BaseBucketAggregation
	{
		private readonly string _path;

		public NestedBucketAggregation(string name, string path)
			: base("nested", name)
		{
			_path = path;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("path", _path, elasticsearchCrudJsonWriter);
		}
	}
}