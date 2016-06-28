using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Query search which can do loads; see the documentation:
	/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html
	/// </summary>
	public class QueryStringQuery : IQuery
	{
		private readonly string _queryString;

		public QueryStringQuery(string queryString)
		{
			_queryString = queryString;
		}

		private double _boost;
		private bool _boostSet;
		private string _defaultField;
		private bool _defaultFieldSet;
		private QueryDefaultOperator _defaultOperator;
		private bool _defaultOperatorSet;
		private string _analyzer;
		private bool _analyzerSet;
		private bool _allowLeadingWildcard;
		private bool _allowLeadingWildcardSet;
		private double _fuzziness;
		private bool _fuzzinessSet;
		private bool _lowercaseExpandedTerms;
		private bool _lowercaseExpandedTermsSet;
		private bool _enablePositionIncrements;
		private bool _enablePositionIncrementsSet;
		private int _fuzzyMaxExpansions;
		private bool _fuzzyMaxExpansionsSet;
		private string _minimumShouldMatch;
		private bool _minimumShouldMatchSet;
		private int _fuzzyPrefixLength;
		private bool _fuzzyPrefixLengthSet;
		private int _phraseSlop;
		private bool _phraseSlopSet;
		private bool _analyzeWildcard;
		private bool _analyzeWildcardSet;
		private bool _autoGeneratePhraseQueries;
		private bool _autoGeneratePhraseQueriesSet;
		private bool _lenient;
		private bool _lenientSet;
		private string _locale;
		private bool _localeSet;
		private List<string> _fields;
		private bool _fieldsSet;
		private bool _useDisMax;
		private bool _useDisMaxSet;
		private int _tieBreaker;
		private bool _tieBreakerSet;

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
		/// default_field
		/// The default field for query terms if no prefix field is specified. Defaults to the index.query.default_field index settings, which in turn defaults to _all.
		/// 
		/// When not explicitly specifying the field to search on in the query string syntax, the index.query.
		/// default_field will be used to derive which field to search on. It defaults to _all field.
		/// 
		/// So, if _all field is disabled, it might make sense to change it to set a different default fie
		/// </summary>
		public string DefaultField
		{
			get { return _defaultField; }
			set
			{
				_defaultField = value;
				_defaultFieldSet = true;
			}
		}

		/// <summary>
		/// default_operator
		/// The default operator used if no explicit operator is specified. For example, with a default operator of OR, the query capital of Hungary is translated to capital OR of OR Hungary, 
		/// and with default operator of AND, the same query is translated to capital AND of AND Hungary. The default value is OR.
		/// </summary>
		public QueryDefaultOperator DefaultOperator
		{
			get { return _defaultOperator; }
			set
			{
				_defaultOperator = value;
				_defaultOperatorSet = true;
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

		/// <summary>
		/// allow_leading_wildcard
		/// When set, * or ? are allowed as the first character. Defaults to true.
		/// </summary>
		public bool AllowLeadingWildcard
		{
			get { return _allowLeadingWildcard; }
			set
			{
				_allowLeadingWildcard = value;
				_allowLeadingWildcardSet = true;
			}
		}
		
		/// <summary>
		/// lowercase_expanded_terms
		/// Whether terms of wildcard, prefix, fuzzy, and range queries are to be automatically lower-cased or not (since they are not analyzed). Default it true.
		/// </summary>
		public bool LowercaseExpandedTerms
		{
			get { return _lowercaseExpandedTerms; }
			set
			{
				_lowercaseExpandedTerms = value;
				_lowercaseExpandedTermsSet = true;
			}
		}

		/// <summary>
		/// enable_position_increments
		/// Set to true to enable position increments in result queries. Defaults to true.
		/// </summary>
		public bool EnablePositionIncrements
		{
			get { return _enablePositionIncrements; }
			set
			{
				_enablePositionIncrements = value;
				_enablePositionIncrementsSet = true;
			}
		}

		/// <summary>
		/// fuzzy_max_expansions
		/// Controls the number of terms fuzzy queries will expand to. Defaults to 50
		/// </summary>
		public int FuzzyMaxExpansions
		{
			get { return _fuzzyMaxExpansions; }
			set
			{
				_fuzzyMaxExpansions = value;
				_fuzzyMaxExpansionsSet = true;
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

		/// <summary>
		/// fuzzy_prefix_length
		/// Set the prefix length for fuzzy queries. Default is 0.
		/// </summary>
		public int FuzzyPrefixLength
		{
			get { return _fuzzyPrefixLength; }
			set
			{
				_fuzzyPrefixLength = value;
				_fuzzyPrefixLengthSet = true;
			}
		}

		/// <summary>
		/// phrase_slop
		/// Sets the default slop for phrases. If zero, then exact phrase matches are required. Default value is 0.
		/// </summary>
		public int PhraseSlop
		{
			get { return _phraseSlop; }
			set
			{
				_phraseSlop = value;
				_phraseSlopSet = true;
			}
		}

		/// <summary>
		/// analyze_wildcard
		/// By default, wildcards terms in a query string are not analyzed. By setting this value to true, a best effort will be made to analyze those as well.
		/// </summary>
		public bool AnalyzeWildcard
		{
			get { return _analyzeWildcard; }
			set
			{
				_analyzeWildcard = value;
				_analyzeWildcardSet = true;
			}
		}

		/// <summary>
		/// auto_generate_phrase_queries
		/// Default to false
		/// </summary>
		public bool AutoGeneratePhraseQueries
		{
			get { return _autoGeneratePhraseQueries; }
			set
			{
				_autoGeneratePhraseQueries = value;
				_autoGeneratePhraseQueriesSet = true;
			}
		}

		/// <summary>
		/// lenient
		/// If set to true will cause format based failures (like providing text to a numeric field) to be ignored.
		/// </summary>
		public bool Lenient
		{
			get { return _lenient; }
			set
			{
				_lenient = value;
				_lenientSet = true;
			}
		}

		/// <summary>
		/// _locale
		/// Locale that should be used for string conversions. Defaults to ROOT.
		/// </summary>
		public string Locale
		{
			get { return _locale; }
			set
			{
				_locale = value;
				_localeSet = true;
			}
		}

		public List<string> Fields
		{
			get { return _fields; }
			set
			{
				_fields = value;
				_fieldsSet = true;
			}
		}
		
		/// <summary>
		/// use_dis_max
		/// Should the queries be combined using dis_max (set it to true), or a bool query (set it to false). Defaults to true.
		/// </summary>
		public bool UseDisMax
		{
			get { return _useDisMax; }
			set
			{
				_useDisMax = value;
				_useDisMaxSet = true;
			}
		}
		
		/// <summary>
		/// tie_breaker
		/// When using dis_max, the disjunction max tie breaker. Defaults to 0
		/// </summary>
		public int TieBreaker
		{
			get { return _tieBreaker; }
			set
			{
				_tieBreaker = value;
				_tieBreakerSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query_string");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("query", _queryString, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("default_field", _defaultField, elasticsearchCrudJsonWriter, _defaultFieldSet);
			JsonHelper.WriteValue("default_operator", _defaultOperator.ToString(), elasticsearchCrudJsonWriter, _defaultOperatorSet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
			JsonHelper.WriteValue("allow_leading_wildcard", _allowLeadingWildcard, elasticsearchCrudJsonWriter, _allowLeadingWildcardSet);
			JsonHelper.WriteValue("lowercase_expanded_terms", _lowercaseExpandedTerms, elasticsearchCrudJsonWriter, _lowercaseExpandedTermsSet);
			JsonHelper.WriteValue("enable_position_increments", _enablePositionIncrements, elasticsearchCrudJsonWriter, _enablePositionIncrementsSet);
			JsonHelper.WriteValue("fuzzy_max_expansions", _fuzzyMaxExpansions, elasticsearchCrudJsonWriter, _fuzzyMaxExpansionsSet);
			JsonHelper.WriteValue("fuzziness", _fuzziness, elasticsearchCrudJsonWriter, _fuzzinessSet);
			JsonHelper.WriteValue("fuzzy_prefix_length", _fuzzyPrefixLength, elasticsearchCrudJsonWriter, _fuzzyPrefixLengthSet);
			JsonHelper.WriteValue("phrase_slop", _phraseSlop, elasticsearchCrudJsonWriter, _phraseSlopSet);
			JsonHelper.WriteValue("analyze_wildcard", _analyzeWildcard, elasticsearchCrudJsonWriter, _analyzeWildcardSet);
			JsonHelper.WriteValue("lenient", _lenient, elasticsearchCrudJsonWriter, _lenientSet);
			JsonHelper.WriteValue("locale", _locale, elasticsearchCrudJsonWriter, _localeSet);
			JsonHelper.WriteValue("auto_generate_phrase_queries", _autoGeneratePhraseQueries, elasticsearchCrudJsonWriter, _autoGeneratePhraseQueriesSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			JsonHelper.WriteValue("minimum_should_match", _minimumShouldMatch, elasticsearchCrudJsonWriter, _minimumShouldMatchSet);

			JsonHelper.WriteListValue("fields", _fields, elasticsearchCrudJsonWriter, _fieldsSet);
			JsonHelper.WriteValue("use_dis_max", _useDisMax, elasticsearchCrudJsonWriter, _useDisMaxSet);
			JsonHelper.WriteValue("tie_breaker", _tieBreaker, elasticsearchCrudJsonWriter, _tieBreakerSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}

