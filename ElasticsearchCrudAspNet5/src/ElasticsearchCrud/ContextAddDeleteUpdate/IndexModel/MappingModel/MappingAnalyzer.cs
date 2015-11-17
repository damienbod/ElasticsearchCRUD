namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel
{
	/// <summary>
	/// The _analyzer mapping allows to use a document field property as the name of the analyzer that will be used to index the document. 
	/// The analyzer will be used for any field that does not explicitly defines an analyzer or index_analyzer when indexing.
	/// {
	///	 "type1" : {
	///		"_analyzer" : {
	///			"path" : "my_field"
	///		}
	///   }
	/// }
	/// 
	/// The above will use the value of the my_field to lookup an analyzer registered under it. For example, indexing the following doc:
	/// {
	///  	"my_field" : "whitespace"
	///  }
	/// </summary>
	public class MappingAnalyzer
	{
		private string _path;
		private bool _pathSet;

		public string Path
		{
			get { return _path; }
			set
			{
				_path = value;
				_pathSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_pathSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_analyzer");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("path");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_path);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}