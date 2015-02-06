using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets
{
	public class NamedBucket
	{
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