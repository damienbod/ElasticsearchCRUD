using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class RangesBucketAggregationsResult : AggregationResult<RangesBucketAggregationsResult>
	{
		public List<RangeBucket> Buckets { get; set; }

		public override RangesBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<RangesBucketAggregationsResult>();
		}
	}
}