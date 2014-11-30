namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel
{
	public class MappingDefinition
	{
		public MappingDefinition()
		{
			RoutingDefinition = new RoutingDefinition();
			All = new MappingAll();
			Source = new MappingSource();
			Analyzer = new MappingAnalyzer();
		}

		public string Index { get; set; }

		public RoutingDefinition RoutingDefinition { get; set; }

		public MappingAll All{ get; set; }

		public MappingSource Source { get; set; }

		public MappingAnalyzer Analyzer { get; set; }
	}
}