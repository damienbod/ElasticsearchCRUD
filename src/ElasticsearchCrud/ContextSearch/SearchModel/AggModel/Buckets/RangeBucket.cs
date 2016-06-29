using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets
{
	/// <summary>
	/// "key":"2.0-1.5","from":2.0,"from_as_string":"2.0","to":1.5,"to_as_string":"1.5","doc_count":0
	/// </summary>
	public class RangeBucket : BaseBucket
	{
		[JsonProperty("key")]
		public object Key { get; set; }

		[JsonProperty("from")]
		public object From { get; set; }

		[JsonProperty("from_as_string")]
		public string FromAsString { get; set; }

		[JsonProperty("to")]
		public object To { get; set; }

		[JsonProperty("to_as_string")]
		public string ToAsString { get; set; }
	}
}