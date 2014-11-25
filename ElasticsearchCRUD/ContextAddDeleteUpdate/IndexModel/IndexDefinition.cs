using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class IndexDefinition : MappingDefinition
	{
		public IndexDefinition()
		{
			// settings the default values
			IndexSettings = new IndexSettings {number_of_replicas = 1, number_of_shards = 5};
		}

		public IndexSettings IndexSettings { get; set; }

	}

	public class MappingDefinition
	{
		public MappingDefinition()
		{
			RoutingDefinition = new RoutingDefinition();
		}

		public string Index { get; set; }
		public RoutingDefinition RoutingDefinition { get; set; }
	}
}
