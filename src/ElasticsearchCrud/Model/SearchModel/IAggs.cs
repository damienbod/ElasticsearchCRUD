namespace ElasticsearchCRUD.Model.SearchModel
{
	public interface IAggs
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}