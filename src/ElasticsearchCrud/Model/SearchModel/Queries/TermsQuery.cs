using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class TermsQuery : IQuery
	{
		private readonly string _term;
		private readonly List<object> _termValues;
		private double _boost;
		private bool _boostSet;

		public TermsQuery(string term, List<object> termValues)
		{
			_term = term;
			_termValues = termValues;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("terms");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteListValue(_term, _termValues, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}