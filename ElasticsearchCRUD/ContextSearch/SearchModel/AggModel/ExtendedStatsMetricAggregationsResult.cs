using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class ExtendedStatsMetricAggregationsResult : AggregationResult<ExtendedStatsMetricAggregationsResult>
	{
		public double Count { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }
		public double Avg { get; set; }
		public double Sum { get; set; }

		[JsonProperty("sum_of_squares")]
		public double SumOfSquares { get; set; }
		public double Variance { get; set; }

		[JsonProperty("std_deviation")]
		public double StdDeviation { get; set; }
	
		/// <summary>
		/// support from version 1.4.3
		/// The standard deviation and its bounds are displayed by default, but they are not always applicable to all data-sets. 
		/// Your data must be normally distributed for the metrics to make sense. The statistics behind standard deviations assumes normally distributed data, 
		/// so if your data is skewed heavily left or right, the value returned will be misleading.
		/// </summary>
		[JsonProperty("std_deviation_bounds")]
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
		public override ExtendedStatsMetricAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<ExtendedStatsMetricAggregationsResult>();
		}
	}
}