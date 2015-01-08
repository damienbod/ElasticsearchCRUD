namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IFilterHolder
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}