namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// A family of match queries that accept text/numerics/dates, analyzes it, and constructs a query out of it. For example:
	/// </summary>
	public class MatchQuery : MatchBase, IQuery
	{
		public MatchQuery(string field, string text) : base(field, text)
		{
		}

		//{
		// "query" : {
		//	  "match" : {
		//		"name" : {
		//			"query" : "group"
		//		}
		//	  }
		//  }
		//}
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("match");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(Field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteBasePropertiesJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
