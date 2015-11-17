using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model;
using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class OptimizeResult
	{
		[JsonProperty(PropertyName = "_shards")]
		public Shards Shards { get; set; }
	}
}