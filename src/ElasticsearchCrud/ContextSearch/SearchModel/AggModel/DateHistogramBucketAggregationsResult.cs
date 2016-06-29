using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class DateHistogramBucketAggregationsResult : AggregationResult<DateHistogramBucketAggregationsResult>
	{
		public List<DateHistogramBucket> Buckets { get; set; }

		public override DateHistogramBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<DateHistogramBucketAggregationsResult>();
		}

	}
}