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
		[JsonProperty(PropertyName = "total")]
		public int Total { get; set; }

		[JsonProperty(PropertyName = "max_score", NullValueHandling = NullValueHandling.Ignore)]
		public double MaxScore { get; set; }

		[JsonProperty(PropertyName = "hits")]
		public Hit<T>[] HitsResult { get; set; }
	}
}