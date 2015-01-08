namespace ElasticsearchCRUD.Model.SearchModel
{
	public class Query : IQueryContainer
	{
		private readonly IQuery _query;

		public Query(IQuery query)
		{
			_query = query;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_query.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		public override string ToString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			WriteJson(elasticsearchCrudJsonWriter);
			return elasticsearchCrudJsonWriter.GetJsonString();
		}
	}
}
