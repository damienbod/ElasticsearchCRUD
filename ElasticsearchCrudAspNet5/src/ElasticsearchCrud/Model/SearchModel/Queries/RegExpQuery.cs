using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The regexp query allows you to use regular expression term queries. See Regular expression syntax for details of the supported regular expression language. 
	/// The "term queries" in that first sentence means that Elasticsearch will apply the regexp to the terms produced by the tokenizer for that field, 
	/// and not to the original text of the field.
	/// 
	/// Note: The performance of a regexp query heavily depends on the regular expression chosen. 
	/// Matching everything like .* is very slow as well as using lookaround regular expressions. 
	/// If possible, you should try to use a long prefix before your regular expression starts. Wildcard matchers like .*?+ will mostly lower performance.
	/// 
	/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/query-dsl-regexp-query.html
	/// </summary>
	public class RegExpQuery : IQuery
	{
		private readonly string _field;
		private readonly string _regularExpression;
		private RegExpFlags _flags;
		private bool _flagsSet;
		private double _boost;
		private bool _boostSet;
		private uint _maxDeterminizedStates;
		private bool _maxDeterminizedStatesSet;

		public RegExpQuery(string field, string regularExpression)
		{
			_field = field;
			_regularExpression = regularExpression;
		}

		public RegExpFlags Flags
		{
			get { return _flags; }
			set
			{
				_flags = value;
				_flagsSet = true;
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
		/// max_determinized_states
		/// </summary>
		public uint MaxDeterminizedStates
		{
			get { return _maxDeterminizedStates; }
			set
			{
				_maxDeterminizedStates = value;
				_maxDeterminizedStatesSet = true;
			}
		}
		 
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("regexp");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("value", _regularExpression, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("flags", _flags.ToString(), elasticsearchCrudJsonWriter, _flagsSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			JsonHelper.WriteValue("max_determinized_states", _maxDeterminizedStates, elasticsearchCrudJsonWriter, _maxDeterminizedStatesSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

}
