namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IQueryContainer
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}