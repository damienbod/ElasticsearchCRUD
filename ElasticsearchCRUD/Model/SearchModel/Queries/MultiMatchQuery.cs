using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class MultiMatchQuery : MatchBase, IQuery
	{
		private readonly string _text;
		private List<string> _fields;
		private bool _fieldsSet;
		private MultiMatchType _multiMatchType;
		private bool _multiMatchTypeSet;
		private double _tieBreaker;
		private bool _tieBreakerSet;

		public MultiMatchQuery(string text) : base(text)
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
		/// 
		/// The best_fields type is most useful when you are searching for multiple words best found in the same field. For instance “brown fox” in a single field is more 
		/// meaningful than “brown” in one field and “fox” in the other.
		/// The best_fields type generates a match query for each field and wraps them in a dis_max query, to find the single best matching field. 
		/// 
		/// Normally the best_fields type uses the score of the single best matching field, but if tie_breaker is specified, then it calculates the score as follows:
		/// the score from the best matching field plus tie_breaker * _score for all other matching fields 
		/// </summary>
		best_fields,
		
		/// <summary>
		/// Finds documents which match any field and combines the _score from each field. See most_fields.
		/// 
		/// The most_fields type is most useful when querying multiple fields that contain the same text analyzed in different ways. For instance, 
		/// the main field may contain synonyms, stemming and terms without diacritics. A second field may contain the original terms, and a third field might contain shingles. 
		/// By combining scores from all three fields we can match as many documents as possible with the main field, 
		/// but use the second and third fields to push the most similar results to the top of the list.
		/// </summary>
		most_fields,
		
		/// <summary>
		/// Treats fields with the same analyzer as though they were one big field. Looks for each word in any field. See cross_fields.
		/// 
		/// The cross_fields type is particularly useful with structured documents where multiple fields should match. 
		/// For instance, when querying the first_name and last_name fields for “Will Smith”, the best match is likely to have “Will” in one field and “Smith” in the other.
		/// 
		/// One way of dealing with these types of queries is simply to index the first_name and last_name fields into a single full_name field. Of course, this can only be done at index time.
		/// 
		/// The cross_field type tries to solve these problems at query time by taking a term-centric approach. It first analyzes the query string into individual terms, 
		/// then looks for each term in any of the fields, as though they were one big field.
		/// </summary>
		cross_fields,
		
		/// <summary>
		/// Runs a match_phrase query on each field and combines the _score from each field. See phrase and phrase_prefix.
		/// 
		/// The phrase and phrase_prefix types behave just like best_fields, but they use a match_phrase or match_phrase_prefix query instead of a match query.
		/// </summary>
		phrase,
		
		/// <summary>
		/// Runs a match_phrase_prefix query on each field and combines the _score from each field. See phrase and phrase_prefix. 
		/// 
		/// The phrase and phrase_prefix types behave just like best_fields, but they use a match_phrase or match_phrase_prefix query instead of a match query.
		/// </summary>
		phrase_prefix

	}
}
