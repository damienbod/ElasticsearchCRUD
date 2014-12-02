using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextSearch.SearchModel
{
	//  "hits":{
	//    "total":1,
	//    "max_score":1.0,
	//    "hits":[{
	//      "_index":"parentdocuments",
	//      "_type":"childdocumentleveltwo",
	//      "_id":"35","_score":1.0,
	//      "_source":{"id":35,"d3":"p8.p25.p35"}
	//     }]
	//   }
	// }
	public class Hits<T>
	{
		public Hits()
		{
			HitsResult = new List<Hit<T>>();
		}

		[JsonProperty(PropertyName = "total")]
		public int Total { get; set; }

		[JsonProperty(PropertyName = "max_score")]
		public double MaxScore { get; set; }

		[JsonProperty(PropertyName = "hits", NullValueHandling= NullValueHandling.Include)]
		public List<Hit<T>> HitsResult { get; set; }
	}
}