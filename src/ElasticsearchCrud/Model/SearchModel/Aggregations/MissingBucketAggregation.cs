using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A field data based single bucket aggregation, that creates a bucket of all documents in the current document set context that are missing a field value 
	/// (effectively, missing a field or having the configured NULL value set). 
	/// This aggregator will often be used in conjunction with other field data bucket aggregators (such as ranges) to return information for all the documents 
	/// that could not be placed in any of the other buckets due to missing field data values.
	/// </summary>
	public class MissingBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;

		public MissingBucketAggregation(string name, string field) : base("missing", name)
		{
			_field = field;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("field", _field, elasticsearchCrudJsonWriter);
		}
	}
}