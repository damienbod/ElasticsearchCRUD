using System.Net;

namespace ElasticsearchCRUD
{
	public class ResultDetails<T>
	{
		public HttpStatusCode Status { get; set; }
		public long TotalHits { get; set; }
		public string RequestBody { get; set; }
		public string RequestUrl { get; set; }
		public string Description { get; set; }
		public T PayloadResult{ get; set; }
		public string ScrollId { get; set; }
	}
}
