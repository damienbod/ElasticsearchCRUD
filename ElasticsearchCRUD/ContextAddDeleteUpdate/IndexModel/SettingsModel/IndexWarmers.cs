using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class IndexWarmers
	{
		public IndexWarmers()
		{
			Warmers = new List<IndexWarmer>();
		}

		public List<IndexWarmer> Warmers { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("warmers");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			foreach (var warmer in Warmers)
			{
				warmer.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}