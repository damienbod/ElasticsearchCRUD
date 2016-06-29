using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	//"aliases" : {
	//  "april_2014" : {},
	//  "year_2014" : {}
	//},
	public class IndexAliases
	{
		public IndexAliases()
		{
			Aliases = new List<IndexAlias>();
		}

		public List<IndexAlias> Aliases { get; set; }

		//"aliases" : { 
		//  "april_2014" : {},
		//  "year_2014" : {}
		//}, 
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("aliases");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			foreach (var alias in Aliases)
			{
				alias.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}