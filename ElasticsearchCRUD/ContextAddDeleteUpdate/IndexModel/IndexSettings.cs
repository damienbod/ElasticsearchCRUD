namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class IndexSettings
	{
		public IndexSettings()
		{
			number_of_shards = 5;
			number_of_replicas = 1;
		}
		public int number_of_shards { get; set; }
		public int number_of_replicas { get; set; }
	}
}
