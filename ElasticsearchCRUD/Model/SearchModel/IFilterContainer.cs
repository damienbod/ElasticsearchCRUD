namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IFilterContainer
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}