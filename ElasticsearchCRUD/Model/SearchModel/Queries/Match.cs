using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// A family of match queries that accept text/numerics/dates, analyzes it, and constructs a query out of it. For example:
	/// </summary>
	public class Match : IQuery
	{
		private readonly string _field;
		private readonly string _text;
		private Operator _operator;
		private bool _operatorSet;
		private ZeroTermsQuery _zeroTermsQuery;
		private bool _zeroTermsQuerySet;
		private double _cutoffFrequency;
		private bool _cutoffFrequencySet;
		private string _analyzer;
		private bool _analyzerSet;

		public Match(string field, string text)
		{
			_field = field;
			_text = text;
		}

		public Operator Operator
		{
			get { return _operator; }
			set
			{
				_operator = value;
				_operatorSet = true;
			}
		}

		/// <summary>
		/// zero_terms_query
		/// If the analyzer used removes all tokens in a query like a stop filter does, the default behavior is to match no documents at all. 
		/// In order to change that the zero_terms_query option can be used, which accepts none (default) and all which corresponds to a match_all query.
		/// </summary>
		public ZeroTermsQuery ZeroTermsQuery
		{
			get { return _zeroTermsQuery; }
			set
			{
				_zeroTermsQuery = value;
				_zeroTermsQuerySet = true;
			}
		}

		/// <summary>
		/// cutoff_frequency range [0..1) 
		/// 
		/// The match query supports a cutoff_frequency that allows specifying an absolute or relative document frequency where high frequency terms 
		/// are moved into an optional subquery and are only scored 
		/// if one of the low frequency (below the cutoff) terms in the case of an or operator or all of the low frequency terms in the case of an and operator match.
		/// 
		/// This query allows handling stopwords dynamically at runtime, is domain independent and doesn’t require on a stopword file. 
		/// It prevent scoring / iterating high frequency terms and only takes the terms into account if a more significant / lower frequency terms match a document. 
		/// Yet, if all of the query terms are above the given cutoff_frequency the query is automatically transformed into a pure conjunction (and) query to ensure fast execution.
		/// 
		/// The cutoff_frequency can either be relative to the number of documents in the index if in the range [0..1) or absolute if greater or equal to 1.0.
		/// </summary>
		public double CutoffFrequency
		{
			get { return _cutoffFrequency; }
			set
			{
				_cutoffFrequency = value;
				if (value <= 0) throw new ElasticsearchCrudException("_cutoffFrequency must be larger than 0");
				if (value > 1) throw new ElasticsearchCrudException("_cutoffFrequency must be equal or smaller than 1.0");
				_cutoffFrequencySet = true;
			}
		}

		/// <summary>
		/// analyzer
		/// The analyzer can be set to control which analyzer will perform the analysis process on the text. 
		/// It default to the field explicit mapping definition, or the default search analyzer, for example:
		/// </summary>
		public string Analyzer
		{
			get { return _analyzer; }
			set
			{
				_analyzer = value;
				_analyzerSet = true;
			}
		}

		//{
		// "query" : {
		//	  "match" : {
		//		"name" : {
		//			"query" : "group"
		//		}
		//	  }
		//  }
		//}
		public virtual void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("match");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("query", _text, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("operator", _operator.ToString(), elasticsearchCrudJsonWriter, _operatorSet);
			JsonHelper.WriteValue("zero_terms_query", _zeroTermsQuery.ToString(), elasticsearchCrudJsonWriter, _zeroTermsQuerySet);
			JsonHelper.WriteValue("cutoff_frequency", _cutoffFrequency, elasticsearchCrudJsonWriter, _cutoffFrequencySet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum ZeroTermsQuery
	{
		all,
		none
	}

	public enum Operator
	{
		and,
		or
	}
	
}
