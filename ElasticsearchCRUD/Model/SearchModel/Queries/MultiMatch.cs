using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class MultiMatch : MatchBase, IQuery
	{
		private readonly string _text;
		private List<string> _fields;
		private bool _fieldsSet;
		private MultiMatchType _multiMatchType;
		private bool _multiMatchTypeSet;
		private double _tieBreaker;
		private bool _tieBreakerSet;

		public MultiMatch(string text) : base(text)
		{
			_text = text;
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
		/// type 
		/// see MultiMatchType for possible values
		/// </summary>
		public MultiMatchType MultiMatchType
		{
			get { return _multiMatchType; }
			set
			{
				_multiMatchType = value;
				_multiMatchTypeSet = true;
			}
		}

		/// <summary>
		/// tie_breaker
		/// By default, each per-term blended query will use the best score returned by any field in a group, then these scores are added together to give the final score. 
		/// The tie_breaker parameter can change the default behaviour of the per-term blended queries. It accepts:
		/// 
		/// 0.0 Take the single best score out of (eg) first_name:will and last_name:will (default)
		/// 1.0 Add together the scores for (eg) first_name:will and last_name:will
		/// 0.0 between 1.0 Take the single best score plus tie_breaker multiplied by each of the scores from other matching fields. 
		/// </summary>
		public double TieBreaker
		{
			get { return _tieBreaker; }
			set
			{
				_tieBreaker = value;
				if (value < 0) throw new ElasticsearchCrudException("TieBreakermust be larger than 0");
				if (value > 1) throw new ElasticsearchCrudException("TieBreaker must be equal or smaller than 1.0");
				_tieBreakerSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("multi_match");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("query", _text, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("fields", _fields, elasticsearchCrudJsonWriter, _fieldsSet);
			JsonHelper.WriteValue("type", _multiMatchType.ToString(), elasticsearchCrudJsonWriter, _multiMatchTypeSet);
			JsonHelper.WriteValue("tie_breaker", _tieBreaker, elasticsearchCrudJsonWriter, _tieBreakerSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			
		}
	}

	public enum MultiMatchType
	{
		/// <summary>
		/// (default) Finds documents which match any field, but uses the _score from the best field. See best_fields.
		/// </summary>
		best_fields,
		
		/// <summary>
		/// Finds documents which match any field and combines the _score from each field. See most_fields.
		/// </summary>
		most_fields,
		
		/// <summary>
		/// Treats fields with the same analyzer as though they were one big field. Looks for each word in any field. See cross_fields.

		/// </summary>
		cross_fields,
		
		/// <summary>
		/// Runs a match_phrase query on each field and combines the _score from each field. See phrase and phrase_prefix.
		/// </summary>
		phrase,
		
		/// <summary>
		/// Runs a match_phrase_prefix query on each field and combines the _score from each field. See phrase and phrase_prefix. 
		/// </summary>
		phrase_prefix

	}
}
