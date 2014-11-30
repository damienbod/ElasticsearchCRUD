using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class IndexDefinition : MappingDefinition
	{
		public IndexDefinition()
		{
			// settings the default values
			IndexSettings = new IndexSettings {NumberOfReplicas = 1, NumberOfShards = 5};
		}

		public IndexSettings IndexSettings { get; set; }

	}
}
