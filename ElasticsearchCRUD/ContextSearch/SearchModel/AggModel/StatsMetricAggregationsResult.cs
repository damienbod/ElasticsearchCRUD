using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class StatsMetricAggregationsResult : AggregationResult<StatsMetricAggregationsResult>
	{
		public double Count { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }
		public double Avg { get; set; }
		public double Sum { get; set; }

		//   "count": 9,
		//   "min": 72,
		//   "max": 99,
		//   "avg": 86,
		//   "sum": 774,
		public override StatsMetricAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<StatsMetricAggregationsResult>();
		}
	}
}

