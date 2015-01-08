namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IQueryHolder
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}