using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// A query that uses the SimpleQueryParser to parse its context. Unlike the regular query_string query, 
	/// the simple_query_string query will never throw an exception, and discards invalid parts of the query
	/// 
	/// The simple_query_string supports the following special characters:
	///
    /// + signifies AND operation
    /// | signifies OR operation
    /// - negates a single token
    /// " wraps a number of tokens to signify a phrase for searching
    /// * at the end of a term signifies a prefix query
    /// ( and ) signify precedence
    /// ~N after a word signifies edit distance (fuzziness)
    /// ~N after a phrase signifies slop amount 
	/// </summary>
	public class SimpleQueryStringQuery :IQuery
	{
		private QueryDefaultOperator _defaultOperator;
		private bool _defaultOperatorSet;
		private string _analyzer;
		private bool _analyzerSet;
		private bool _lowercaseExpandedTerms;
		private bool _lowercaseExpandedTermsSet;

		private bool _lenient;
		private bool _lenientSet;
		private string _locale;
		private bool _localeSet;
		private List<string> _fields;
		private bool _fieldsSet;
		private readonly string _queryString;
		private SimpleQueryFlags _flags;
		private bool _flagsSet;

		public SimpleQueryStringQuery(string queryString)
		{
			_queryString = queryString;
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

		public SimpleQueryFlags Flags
		{
			get { return _flags; }
			set
			{
				_flags = value;
				_flagsSet = true;
			}
		}
//flags
//Flags specifying which features of the simple_query_string to enable. Defaults to ALL.




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

		/// <summary>
		/// fields
		/// The fields to perform the parsed query against. Defaults to the index.query.default_field index settings, which in turn defaults to _all.
		/// </summary>
		public List<string> Fields
		{
			get { return _fields; }
			set
			{
				_fields = value;
				_fieldsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("simple_query_string");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("query", _queryString, elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("default_operator", _defaultOperator.ToString(), elasticsearchCrudJsonWriter, _defaultOperatorSet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
			JsonHelper.WriteValue("lowercase_expanded_terms", _lowercaseExpandedTerms, elasticsearchCrudJsonWriter, _lowercaseExpandedTermsSet);
			JsonHelper.WriteValue("lenient", _lenient, elasticsearchCrudJsonWriter, _lenientSet);
			JsonHelper.WriteValue("flags", _flags.ToString(), elasticsearchCrudJsonWriter, _flagsSet);
			JsonHelper.WriteValue("locale", _locale, elasticsearchCrudJsonWriter, _localeSet);
			JsonHelper.WriteListValue("fields", _fields, elasticsearchCrudJsonWriter, _fieldsSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum SimpleQueryFlags
	{
		ALL, 
		NONE, 
		AND, 
		OR, 
		NOT, 
		PREFIX, 
		PHRASE, 
		PRECEDENCE, 
		ESCAPE, 
		WHITESPACE, 
		FUZZY, 
		NEAR,
		SLOP
	}
}
