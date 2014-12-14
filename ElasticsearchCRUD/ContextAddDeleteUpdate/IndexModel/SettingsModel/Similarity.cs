namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class Similarity
	{
		private readonly bool _analyzerSet;
		private readonly string _name;
		private string _type;

		public Similarity(string name)
		{
			_analyzerSet = true;
			_name = name.ToLower();
			_type = "BM25";
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_analyzerSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				// TODO write values

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}