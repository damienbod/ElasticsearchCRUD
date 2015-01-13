using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The fuzzy_like_this_field query is the same as the fuzzy_like_this query, except that it runs against a single field. 
	/// It provides nicer query DSL over the generic fuzzy_like_this query, and support typed fields query 
	/// (automatically wraps typed fields with type filter to match only on the specific type).
	/// </summary>
	public class FuzzyLikeThisFieldQuery : IQuery
	{
		private readonly string _likeThis;
		private readonly string _fieldName;
		private double _boost;
		private bool _boostSet;
		private string _analyzer;
		private bool _analyzerSet;
		private bool _ignoreTf;
		private bool _ignoreTfSet;
		private uint _maxQueryTerms;
		private bool _maxQueryTermsSet;
		private double _fuzziness;
		private bool _fuzzinessSet;
		private int _prefixLength;
		private bool _prefixLengthSet;

		public FuzzyLikeThisFieldQuery(string fieldName, string likeThis)
		{
			_likeThis = likeThis;
			_fieldName = fieldName;
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
		/// ignore_tf
		/// Should term frequency be ignored. Defaults to false.
		/// </summary>
		public bool IgnoreTf
		{
			get { return _ignoreTf; }
			set
			{
				_ignoreTf = value;
				_ignoreTfSet = true;
			}
		}

		/// <summary>
		/// max_query_terms
		/// The maximum number of query terms that will be included in any generated query. Defaults to 25.
		/// </summary>
		public uint MaxQueryTerms
		{
			get { return _maxQueryTerms; }
			set
			{
				_maxQueryTerms = value;
				_maxQueryTermsSet = true;
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
		/// prefix_length
		/// Length of required common prefix on variant terms. Defaults to 0.
		/// </summary>
		public int PrefixLength
		{
			get { return _prefixLength; }
			set
			{
				_prefixLength = value;
				_prefixLengthSet = true;
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

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("fuzzy_like_this_field");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_fieldName);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("like_text", _likeThis, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("ignore_tf", _ignoreTf, elasticsearchCrudJsonWriter, _ignoreTfSet);
			JsonHelper.WriteValue("max_query_terms", _maxQueryTerms, elasticsearchCrudJsonWriter, _maxQueryTermsSet);
			JsonHelper.WriteValue("fuzziness", _fuzziness, elasticsearchCrudJsonWriter, _fuzzinessSet);
			JsonHelper.WriteValue("prefix_length", _prefixLength, elasticsearchCrudJsonWriter, _prefixLengthSet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}