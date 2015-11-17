using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class PercentilesMetricAggregationsResult : AggregationResult<PercentilesMetricAggregationsResult>
	{
		public Dictionary<string,double> Values { get; set; }
		public override PercentilesMetricAggregationsResult GetValueFromJToken(JToken result)
		{
			Values = new Dictionary<string, double>();
			Values = result["values"].ToObject<Dictionary<string, double>>();		
			return this;
		}
	}
}