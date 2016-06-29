using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class FiltersNamedBucketAggregationsResult : AggregationResult<FiltersNamedBucketAggregationsResult>
	{
		[JsonProperty("buckets")]
		public NamedBucket Buckets { get; set; }

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
		public override FiltersNamedBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<FiltersNamedBucketAggregationsResult>();
		}
	}
}