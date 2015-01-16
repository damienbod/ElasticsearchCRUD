namespace ElasticsearchCRUD.Model.SearchModel.Sorting
{
	public interface ISort
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}