using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class StatsAggregationsResult : AggregationResult<StatsAggregationsResult>
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
		public override StatsAggregationsResult GetValueFromJToken(JToken result)
		{
			Count = result["count"].Value<double>();
			Min = result["min"].Value<double>();
			Max = result["max"].Value<double>();
			Avg = result["avg"].Value<double>();
			Sum = result["sum"].Value<double>();
			return this;
		}
	}
}