using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class ReverseNestedBucketAggregation : BaseBucketAggregation
	{
		private string _path;
		private bool _pathSet;

		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				_pathSet = true;
			}
		}

		public ReverseNestedBucketAggregation(string name)
			: base("reverse_nested", name)
		{
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("path", _path, elasticsearchCrudJsonWriter, _pathSet);
		}
	}
}