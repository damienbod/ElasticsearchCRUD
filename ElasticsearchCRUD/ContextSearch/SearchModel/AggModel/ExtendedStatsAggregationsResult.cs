using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class ExtendedStatsAggregationsResult : AggregationResult<ExtendedStatsAggregationsResult>
	{
		public double Count { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }
		public double Avg { get; set; }
		public double Sum { get; set; }
		public double SumOfSquares { get; set; }
		public double Variance { get; set; }
		public double StdDeviation { get; set; }

		// support from version 1.4.3
		public StdDeviationBounds StdDeviationBounds { get; set; }

		//"count": 9,
		//   "min": 72,
		//   "max": 99,
		//   "avg": 86,
		//   "sum": 774,
		//   "sum_of_squares": 67028,
		//   "variance": 51.55555555555556,
		//   "std_deviation": 7.180219742846005,
		//   "std_deviation_bounds": {
		//	"upper": 100.36043948569201,
		//	"lower": 71.63956051430799
		//   }
		public override ExtendedStatsAggregationsResult GetValueFromJToken(JToken result)
		{
			Count = result["count"].Value<double>();
			Min = result["min"].Value<double>();
			Max = result["max"].Value<double>();
			Avg = result["avg"].Value<double>();
			Sum = result["sum"].Value<double>();
			SumOfSquares = result["sum_of_squares"].Value<double>();
			Variance = result["variance"].Value<double>();
			StdDeviation = result["std_deviation"].Value<double>();
			if (result["std_deviation_bounds"] != null)
			{
				StdDeviationBounds = new StdDeviationBounds
				{
					Upper = result["std_deviation_bounds"]["upper"].Value<double>(),
					Lower = result["std_deviation_bounds"]["lower"].Value<double>()
				};
			}
			return this;
		}
	}
}