namespace ElasticsearchCRUD.Model.GeoModel
{
	public interface IGeoType
	{
		string Type { get; set; }
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}