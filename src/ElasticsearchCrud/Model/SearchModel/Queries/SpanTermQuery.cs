using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Matches spans containing a term. The span term query maps to Lucene SpanTermQuery. 
	/// </summary>
	public class SpanTermQuery : ISpanQuery
	{
		private readonly string _field;
		private readonly string _fieldValue;
		private double _boost;
		private bool _boostSet;

		public SpanTermQuery(string field, string fieldValue)
		{
			_field = field;
			_fieldValue = fieldValue;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("span_term");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("term", _fieldValue, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
