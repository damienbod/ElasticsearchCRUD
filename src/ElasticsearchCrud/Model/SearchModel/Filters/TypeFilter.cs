using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// Filters documents matching the provided document / mapping type. 
	/// Note, this filter can work even when the _type field is not indexed (using the _uid field).
	/// </summary>
	public class TypeFilter : IFilter
	{
		private readonly string _type;

		public TypeFilter(string type)
		{
			_type = type;
		}


		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("type");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("value", _type, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}