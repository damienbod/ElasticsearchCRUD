using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets
{
	/// <summary>
	/// "key": "Bicycle theft",
	/// "score": 0.371235374214817,
	/// "bg_count": 66799
	/// "doc_count" : 33
	/// </summary>
	public class SignificantTermsBucket : BaseBucket
	{
		[JsonProperty("key")]
		public object Key { get; set; }

		[JsonProperty("score")]
		public double Score { get; set; }

		[JsonProperty("bg_count")]
		public int BgCount { get; set; }
	}
}

