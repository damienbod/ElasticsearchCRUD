using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class IndexDefinition 
	{
		public IndexDefinition()
		{
			// settings the default values
			IndexSettings = new IndexSettings {NumberOfReplicas = 1, NumberOfShards = 5};
			IndexAliases = new IndexAliases();
			IndexWarmers = new Warmers();
			Mapping = new MappingDefinition();
		}

		public IndexSettings IndexSettings { get; set; }

		public IndexAliases IndexAliases { get; set; }

		public Warmers IndexWarmers{ get; set; }

		/// <summary>
		/// TODO change this to a list so n mappings can be defined
		/// </summary>
		public MappingDefinition Mapping { get; set; }

	}
}
