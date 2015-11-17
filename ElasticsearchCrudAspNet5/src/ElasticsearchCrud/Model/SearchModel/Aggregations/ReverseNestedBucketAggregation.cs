using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A special single bucket aggregation that enables aggregating on parent docs from nested documents. 
	/// Effectively this aggregation can break out of the nested block structure and link to other nested structures or the root document, 
	/// which allows nesting other aggregations that aren’t part of the nested object in a nested aggregation.
	/// 
	/// The reverse_nested aggregation must be defined inside a nested aggregation.
	/// 
	/// Options:
	/// 
	/// path - Which defines to what nested object field should be joined back. The default is empty, which means that it joins back to the root / main document level. 
	/// The path cannot contain a reference to a nested object field that falls outside the nested aggregation’s nested structure a reverse_nested is in. 
	/// </summary>
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