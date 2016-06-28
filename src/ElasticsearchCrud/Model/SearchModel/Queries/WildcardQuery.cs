using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class WildcardQuery : IQuery
	{
		private readonly string _field;
		private readonly object _wildcard;
		private double _boost;
		private bool _boostSet;

		public WildcardQuery(string field, object wildcard)
		{
			_field = field;
			_wildcard = wildcard;
		}

		public double Boost
		{
			get { return _boost; }
			set
			{
				_boost = value;
				_boostSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("wildcard");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("wildcard", _wildcard, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
