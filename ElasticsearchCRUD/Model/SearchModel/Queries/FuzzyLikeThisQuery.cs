using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Fuzzy like this query find documents that are "like" provided text by running it against one or more fields
	/// 
	/// Fuzzifies ALL terms provided as strings and then picks the best n differentiating terms. 
	/// In effect this mixes the behaviour of FuzzyQuery and MoreLikeThis but with special consideration of fuzzy scoring factors. 
	/// This generally produces good results for queries where users may provide details in a number of fields and have no knowledge of boolean query syntax 
	/// and also want a degree of fuzzy matching and a fast query.
	/// 
	/// For each source term the fuzzy variants are held in a BooleanQuery with no coord factor (because we are not looking for matches on multiple variants in any one doc). 
	/// Additionally, a specialized TermQuery is used for variants and does not use that variant term’s IDF because this would favor rarer terms, such as misspellings. 
	/// Instead, all variants use the same IDF ranking (the one for the source query term) and this is factored into the variant’s boost. 
	/// If the source query term does not exist in the index the average IDF of the variants is used.
	/// </summary>
	public class FuzzyLikeThisQuery : IQuery 
	{
		private readonly string _likeThis;
		private double _boost;
		private bool _boostSet;
		private List<string> _fields;
		private bool _fieldsSet;
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

		public FuzzyLikeThisQuery(string likeThis)
		{
			_likeThis = likeThis;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("fuzzy_like_this");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();


			JsonHelper.WriteListValue("fields", _fields, elasticsearchCrudJsonWriter, _fieldsSet);
			JsonHelper.WriteValue("like_text", _likeThis, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("ignore_tf", _ignoreTf, elasticsearchCrudJsonWriter, _ignoreTfSet);
			JsonHelper.WriteValue("max_query_terms", _maxQueryTerms, elasticsearchCrudJsonWriter, _maxQueryTermsSet);
			JsonHelper.WriteValue("fuzziness", _fuzziness, elasticsearchCrudJsonWriter, _fuzzinessSet);
			JsonHelper.WriteValue("prefix_length", _prefixLength, elasticsearchCrudJsonWriter, _prefixLengthSet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
