using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model;
using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextSearch.SearchModel
{
	// {
	//  "took":3,
	//  "timed_out":false,
	//  "_shards":{"total":5,"successful":5,"failed":0},
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
	public class SearchResult<T>
	{
		[JsonProperty(PropertyName = "_scroll_id")]
		public string ScrollId { get; set; }

		[JsonProperty(PropertyName = "took")]
		public int Took { get; set; }

		[JsonProperty(PropertyName = "timed_out")]
		public bool TimedOut { get; set; }

		[JsonProperty(PropertyName = "_shards")]
		public Shards Shards { get; set; }

		[JsonProperty(PropertyName = "hits")]
		public Hits<T> Hits { get; set; }

		[JsonProperty(PropertyName = "aggregations")]
		public Aggregations Aggregations { get; set; }
		
	}
}
