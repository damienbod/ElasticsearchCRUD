using System.Net;

namespace ElasticsearchCRUD
{
	public class ResultDetails<T>
	{
		public HttpStatusCode Status { get; set; }
		public string Description { get; set; }
		public T PayloadResult{ get; set; }
	}
}
