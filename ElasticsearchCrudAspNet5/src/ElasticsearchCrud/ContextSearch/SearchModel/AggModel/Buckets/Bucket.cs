using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets
{
	public class Bucket : BaseBucket
	{
		[JsonProperty("key")]
		public object Key { get; set; }
	}
}