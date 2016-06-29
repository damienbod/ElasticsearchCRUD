using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public abstract class MatchBase 
	{
		private readonly string _text;
		private Operator _operator;
		private bool _operatorSet;
		private ZeroTermsQuery _zeroTermsQuery;
		private bool _zeroTermsQuerySet;
		private double _cutoffFrequency;
		private bool _cutoffFrequencySet;
		private string _analyzer;
		private bool _analyzerSet;
		private double _boost;
		private bool _boostSet;
		private string _minimumShouldMatch;
		private bool _minimumShouldMatchSet;
		private double _fuzziness;
		private bool _fuzzinessSet;

		protected MatchBase(string text)
		{
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

		public double Boost
		{
			get { return _boost; }
			set
			{
				_boost = value;
				_boostSet = true;
			}
		}

		/// <summary>
		/// fuzziness
		/// The minimum similarity of the term variants. Defaults to 0.5. See the section called Fuzziness
		/// </summary>
		public double Fuzziness
		{
			get { return _fuzziness; }
			set
			{
				_fuzziness = value;
				_fuzzinessSet = true;
			}
		}

		/// <summary>
		/// minimum_should_match
		/// The minimum_should_match parameter possible values:
		/// 
		/// Integer : Indicates a fixed value regardless of the number of optional clauses.
		/// Negative integer : Indicates that the total number of optional clauses, minus this number should be mandatory.
		/// Percentage 75% : Indicates that this percent of the total number of optional clauses are necessary. The number computed from the percentage is rounded down and used as the minimum.
		/// Negative percentage -25% Indicates that this percent of the total number of optional clauses can be missing.
		///  The number computed from the percentage is rounded down, before being subtracted from the total to determine the minimum.
		/// Combination : A positive integer, followed by the less-than symbol, followed by any of the previously mentioned specifiers is a conditional specification.
		///  It indicates that if the number of optional clauses is equal to (or less than) the integer, they are all required, but if it’s greater than the integer,
		///  the specification applies. In this example: if there are 1 to 3 clauses they are all required, but for 4 or more clauses only 90% are required.
		/// Multiple combinations :  Multiple conditional specifications can be separated by spaces, each one only being valid for numbers greater than the one before it.
		///  In this example: if there are 1 or 2 clauses both are required, if there are 3-9 clauses all but 25% are required, and if there are more than 9 clauses,
		///  all but three are required.
		///
		/// NOTE:
		/// When dealing with percentages, negative values can be used to get different behavior in edge cases. 75% and -25% mean the same thing when dealing with 4 clauses, 
		/// but when dealing with 5 clauses 75% means 3 are required, but -25% means 4 are required.
		///
		/// If the calculations based on the specification determine that no optional clauses are needed, 
		/// the usual rules about BooleanQueries still apply at search time (a BooleanQuery containing no required clauses must still match at least one optional clause)
		///
		/// No matter what number the calculation arrives at, a value greater than the number of optional clauses, or a value less than 1 will never be used. 
		/// (ie: no matter how low or how high the result of the calculation result is, the minimum number of required matches will never be lower than 1 or greater than the number of clauses.
		/// </summary>
		public string MinimumShouldMatch
		{
			get { return _minimumShouldMatch; }
			set
			{
				_minimumShouldMatch = value;
				_minimumShouldMatchSet = true;
			}
		}
		
		protected void WriteBasePropertiesJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("query", _text, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("operator", _operator.ToString(), elasticsearchCrudJsonWriter, _operatorSet);
			JsonHelper.WriteValue("zero_terms_query", _zeroTermsQuery.ToString(), elasticsearchCrudJsonWriter, _zeroTermsQuerySet);
			JsonHelper.WriteValue("cutoff_frequency", _cutoffFrequency, elasticsearchCrudJsonWriter, _cutoffFrequencySet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			JsonHelper.WriteValue("fuzziness", _fuzziness, elasticsearchCrudJsonWriter, _fuzzinessSet);
			JsonHelper.WriteValue("minimum_should_match", _minimumShouldMatch, elasticsearchCrudJsonWriter, _minimumShouldMatchSet);
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