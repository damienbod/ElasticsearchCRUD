using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class HistogramBucketAggregationsResult : AggregationResult<HistogramBucketAggregationsResult>
	{
		public List<Buckets> Buckets { get; set; }

		public override HistogramBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<HistogramBucketAggregationsResult>();
		}

	}
}