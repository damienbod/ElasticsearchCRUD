using Newtonsoft.Json;

namespace ElasticsearchCRUD.Model
{
	/// <summary>
	/// "_shards":{"total":5,"successful":5,"failed":0}
	/// </summary>
	public class Shards
	{
		[JsonProperty(PropertyName = "total")]
		public int Total { get; set; }

		[JsonProperty(PropertyName = "successful")]
		public int Successful { get; set; }

		[JsonProperty(PropertyName = "failed")]
		public int Failed { get; set; }
	}
}