using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class Warmers
	{
		public Warmers()
		{
			WarmersList = new List<Warmer>();
		}

		public List<Warmer> WarmersList { get; set; }

		//"aliases" : { 
		//  "april_2014" : {},
		//  "year_2014" : {}
		//}, 
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("warmers");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			foreach (var warmer in WarmersList)
			{
				warmer.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}