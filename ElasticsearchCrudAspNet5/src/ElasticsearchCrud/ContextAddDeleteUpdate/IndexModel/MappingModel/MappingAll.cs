namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel
{
	/// <summary>
	/// The idea of the _all field is that it includes the text of one or more other fields within the document indexed. 
	/// It can come very handy especially for search requests, where we want to execute a search query against the content of a document, 
	/// without knowing which fields to search on. This comes at the expense of CPU cycles and index size.
	/// The _all fields can be completely disabled. Explicit field mappings and object mappings can be excluded / included in the _all field. 
	/// By default, it is enabled and all fields are included in it for ease of use.
	/// When disabling the _all field, it is a good practice to set index.query.default_field to a different value 
	/// (for example, if you have a main "message" field in your data, set it to message).
	/// One of the nice features of the _all field is that it takes into account specific fields boost levels. 
	/// Meaning that if a title field is boosted more than content, the title (part) in the _all field will mean more than the content (part) in the _all field.
	/// "_all" : {"enabled" : true}
	/// </summary>
	public class MappingAll
	{
		private bool _enabled;
		private bool _enabledSet;

		public bool Enabled {
			get { return _enabled; }
			set
			{
				_enabled = value;
				_enabledSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_enabledSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_all");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("enabled");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_enabled);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}