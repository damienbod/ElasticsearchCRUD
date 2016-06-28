using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class Aggregations
	{
		[JsonExtensionData]
		public Dictionary<string, JToken> Fields { get; set; }

		public T GetSingleMetricAggregationValue<T>(string name)
		{
			return Fields[name]["value"].Value<T>();
		}

		public T GetComplexValue<T>(string name) where T : AggregationResult<T>
		{
			return Activator.CreateInstance<T>().GetValueFromJToken(Fields[name]);
		}
	}

	public abstract class AggregationResult<T>
	{
		public abstract T GetValueFromJToken(JToken result);
	}
}