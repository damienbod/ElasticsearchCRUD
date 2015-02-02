using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class TopHitsAggregationsResult<T> : AggregationResult<TopHitsAggregationsResult<T>>
	{
		[JsonProperty(PropertyName = "hits")]
		public Hits<T> Hits { get; set; }

		public override TopHitsAggregationsResult<T> GetValueFromJToken(JToken result)
		{
			return result.ToObject<TopHitsAggregationsResult<T>>();
		}
	}
}
