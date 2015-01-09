using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	/// <summary>
	/// As a general rule, queries should be used instead of filters:
	/// 
    /// - for full text search
    /// - where the result depends on a relevance score 
	/// </summary>
	public class Query : IQueryHolder
	{
		private readonly IQuery _query;
		private string _name;
		private bool _nameSet;

		public Query(IQuery query)
		{
			_query = query;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				_nameSet = true;
			}
		}


		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_query.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("_name", _name, elasticsearchCrudJsonWriter, _nameSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		public override string ToString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			WriteJson(elasticsearchCrudJsonWriter);
			return elasticsearchCrudJsonWriter.GetJsonString();
		}
	}
}
