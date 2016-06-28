using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel
{
	//    "hits":[{
	//      "_index":"parentdocuments",
	//      "_type":"childdocumentleveltwo",
	//      "_id":"35",
	//      "_score":1.0,
	//      "_source":{"id":35,"d3":"p8.p25.p35"}
	//     }]
	public class Hit<T>
	{
		[JsonProperty(PropertyName = "_index")]
		public string Index { get; set; }

		[JsonProperty(PropertyName = "_type")]
		public string TypeInIndex { get; set; }

		[JsonProperty(PropertyName = "_id")]
		public object Id { get; set; }

		[JsonProperty(PropertyName = "_score", NullValueHandling = NullValueHandling.Ignore)]
		public double Score { get; set; }
		
		[JsonProperty(PropertyName = "_source", NullValueHandling= NullValueHandling.Ignore)]
		public T Source { get; set; }

		[JsonProperty(PropertyName = "highlight", NullValueHandling = NullValueHandling.Ignore)]
		public Dictionary<string, IEnumerable<string>> Highlights { get; set; }

		public T1 GetSourceFromJToken<T1>()
		{
			var token = Source as JToken;
			return token.ToObject<T1>();
		}

		/// <summary>
		/// This can be used for inner_hits or nested data which is added to the hit result
		/// </summary>
		[JsonExtensionData]
		public Dictionary<string, JToken> ExtensionData { get; set; }
	}
}