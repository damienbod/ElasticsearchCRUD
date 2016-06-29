using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A special single bucket aggregation that enables aggregating from buckets on parent document types to buckets on child documents.
	/// 
	/// This aggregation relies on the _parent field in the mapping. This aggregation has a single option:
	/// 
	/// type - The what child type the buckets in the parent space should be mapped to. 
	/// </summary>
	public class ChidrenBucketAggregation : BaseBucketAggregation
	{
		private readonly string _parentType;

		public ChidrenBucketAggregation(string name, string parentType)
			: base("children", name)
		{
			_parentType = parentType;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", _parentType, elasticsearchCrudJsonWriter);
		}
	}
}