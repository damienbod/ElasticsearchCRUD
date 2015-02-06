using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
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
}