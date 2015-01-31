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
		public StdDeviationBounds StdDeviationBounds { get; set; }

		public override ExtendedStatsAggregationsResult GetValueFromJToken(JToken result)
		{
			// TODO set the values
			return this;
		}
	}
}