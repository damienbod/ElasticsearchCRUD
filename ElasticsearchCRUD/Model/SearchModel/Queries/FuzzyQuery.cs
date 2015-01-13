using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The fuzzy query uses similarity based on Levenshtein edit distance for string fields, and a +/- margin on numeric and date fields.
	/// String fields 
	/// The fuzzy query generates all possible matching terms that are within the maximum edit distance specified in fuzziness and 
	/// then checks the term dictionary to find out which of those generated terms actually exist in the index.
	/// 
	/// Warning: this query can be very heavy if prefix_length and max_expansions are both set to 0. This could cause every term in the index to be examined!
	/// </summary>
	public class FuzzyQuery : IQuery
	{
		private readonly string _fieldName;
		private readonly string _fuzzyValue;
		private double _boost;
		private bool _boostSet;
		private double _fuzziness;
		private bool _fuzzinessSet;
		private int _prefixLength;
		private bool _prefixLengthSet;
		private bool _maxExpansionsSet;
		private uint _maxExpansions;

		public FuzzyQuery(string fieldName, string fuzzyValue)
		{
			_fieldName = fieldName;
			_fuzzyValue = fuzzyValue;
		}

		/// <summary>
		/// max_expansions
		/// The maximum number of terms that the fuzzy query will expand to. Defaults to 50. 
		/// </summary>
		public uint MaxExpansions
		{
			get { return _maxExpansions; }
			set
			{
				_maxExpansions = value;
				_maxExpansionsSet = true;
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


		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("fuzzy");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_fieldName);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("value", _fuzzyValue, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max_expansions", _maxExpansions, elasticsearchCrudJsonWriter, _maxExpansionsSet);
			JsonHelper.WriteValue("fuzziness", _fuzziness, elasticsearchCrudJsonWriter, _fuzzinessSet);
			JsonHelper.WriteValue("prefix_length", _prefixLength, elasticsearchCrudJsonWriter, _prefixLengthSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
