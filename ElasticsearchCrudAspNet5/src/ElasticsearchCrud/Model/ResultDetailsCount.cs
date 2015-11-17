namespace ElasticsearchCRUD.Model
{
	public class ResultDetailsCount<T> : ResultDetails<T>
	{
		public long Count { get; set; }		
	}
}