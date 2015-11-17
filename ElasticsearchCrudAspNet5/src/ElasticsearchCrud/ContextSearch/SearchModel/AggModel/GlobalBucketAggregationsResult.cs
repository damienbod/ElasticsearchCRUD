using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	/// <summary>
	/// Defines a single bucket of all the documents within the search execution context. 
	/// This context is defined by the indices and the document types you’re searching on, but is not influenced by the search query itself.
	/// 
	/// Note
	/// Global aggregators can only be placed as top level aggregators (it makes no sense to embed a global aggregator within another bucket aggregator)
	/// </summary>
	public class GlobalBucketAggregationsResult : AggregationResult<GlobalBucketAggregationsResult>
	{
		[JsonProperty("doc_count")]
		public int DocCount { get; set; }

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

		public override GlobalBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<GlobalBucketAggregationsResult>();
		}

	}
}