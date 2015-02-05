using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class SignificantTermsBucketAggregationsResult : AggregationResult<SignificantTermsBucketAggregationsResult>
	{
		[JsonProperty("doc_count")]
		public int DocCount { get; set; }

		public List<SignificantTermsBucket> Buckets { get; set; }

		public override SignificantTermsBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<SignificantTermsBucketAggregationsResult>();
		}

	}

	/// <summary>
	/// "key": "Bicycle theft",
	/// "score": 0.371235374214817,
	/// "bg_count": 66799
	/// "doc_count" : 33
	/// </summary>
	public class SignificantTermsBucket
	{
		[JsonProperty("key")]
		public object Key { get; set; }

		[JsonProperty("doc_count")]
		public int DocCount { get; set; }

		[JsonProperty("score")]
		public double Score { get; set; }

		[JsonProperty("bg_count")]
		public int BgCount { get; set; }

		[JsonExtensionData]
		public Dictionary<string, JToken> SubAggregations { get; set; }

		public T GetSubAggregationsFromJTokenName<T>(string name)
		{
			return SubAggregations[name].ToObject<T>();
		}

		public T GetSingleMetricSubAggregationValue<T>(string name)
		{
			return SubAggregations[name]["value"].Value<T>();
		}
	}
}