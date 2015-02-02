using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class PercentileRanksAggregationsResult : AggregationResult<PercentileRanksAggregationsResult>
	{
		public Dictionary<string, double> Values { get; set; }
		public override PercentileRanksAggregationsResult GetValueFromJToken(JToken result)
		{
			Values = new Dictionary<string, double>();
			Values = result["values"].ToObject<Dictionary<string, double>>();
			return this;
		}

	}
}