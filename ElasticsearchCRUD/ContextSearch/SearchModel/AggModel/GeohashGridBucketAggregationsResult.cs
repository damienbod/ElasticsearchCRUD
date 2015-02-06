using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class GeohashGridBucketAggregationsResult : AggregationResult<GeohashGridBucketAggregationsResult>
	{
		public List<Bucket> Buckets { get; set; }

		public override GeohashGridBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<GeohashGridBucketAggregationsResult>();
		}

	}
}