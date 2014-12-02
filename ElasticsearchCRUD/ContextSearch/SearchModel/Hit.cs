using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextSearch.SearchModel
{
	//    "hits":[{
	//      "_index":"parentdocuments",
	//      "_type":"childdocumentleveltwo",
	//      "_id":"35","_score":1.0,
	//      "_source":{"id":35,"d3":"p8.p25.p35"}
	//     }]
	public class Hit<T>
	{
		[JsonProperty(PropertyName = "_index")]
		public string Index { get; set; }

		[JsonProperty(PropertyName = "_type")]
		public string TypeInIndex { get; set; }

		[JsonProperty(PropertyName = "_id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "_source")]
		public T Source { get; set; }
	}
}