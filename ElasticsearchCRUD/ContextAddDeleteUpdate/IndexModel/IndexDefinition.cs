namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class IndexDefinition
	{
		public IndexDefinition()
		{
			// settings the default values
			IndexSettings = new IndexSettings {number_of_replicas = 1, number_of_shards = 5};
			RoutingDefinition = new RoutingDefinition();
		}

		public IndexSettings IndexSettings { get; set; }

		public RoutingDefinition RoutingDefinition { get; set; }
	}
}
