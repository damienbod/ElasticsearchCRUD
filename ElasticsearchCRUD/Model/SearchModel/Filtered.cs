using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	public class Filtered : IQuery
	{
		private Query _query;
		private readonly Filter _filter;
		private bool _querySet;

		public Filtered(Filter filter)
		{
			_filter = filter;
		}

		public Query Query
		{
			get { return _query; }
			set
			{
				_query = value;
				_querySet = true;
			}
		}

		// TODO add properties

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filtered");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("query", _query,elasticsearchCrudJsonWriter,_querySet);
			_filter.WriteJson(elasticsearchCrudJsonWriter);

			// TODO properties

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}