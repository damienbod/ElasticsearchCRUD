namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IFilter
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}