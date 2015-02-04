using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class GeohashGridBucketAggregationsResult : AggregationResult<GeohashGridBucketAggregationsResult>
	{
		public List<Buckets> Buckets { get; set; }

		public override GeohashGridBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<GeohashGridBucketAggregationsResult>();
		}

	}
}