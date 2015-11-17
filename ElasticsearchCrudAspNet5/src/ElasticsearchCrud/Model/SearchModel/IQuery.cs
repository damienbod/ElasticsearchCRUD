namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IQuery
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}