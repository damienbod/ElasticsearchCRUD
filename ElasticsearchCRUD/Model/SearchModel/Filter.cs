namespace ElasticsearchCRUD.Model.SearchModel
{
	public class Filter : IFilterHolder
	{
		private readonly IFilter _filter;

		public Filter(IFilter filter)
		{
			_filter = filter;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_filter.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}