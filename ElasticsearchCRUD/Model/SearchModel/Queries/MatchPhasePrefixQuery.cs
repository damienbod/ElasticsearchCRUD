using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The match_phrase_prefix is the same as match_phrase, except that it allows for prefix matches on the last term in the text
	/// </summary>
	public class MatchPhasePrefixQuery : MatchBase, IQuery
	{
		private int _slop;
		private bool _slopSet;
		private int _maxExpansions;
		private bool _maxExpansionsSet;

		private readonly string _field;
		public MatchPhasePrefixQuery(string field, string text)
			: base(text)
		{
			_field = field;
		}

		/// <summary>
		/// slop
		/// 
		/// The slop parameter tells the match_phrase query how far apart terms are allowed to be while still considering the document a match. 
		/// By how far apart we mean how many times do you need to move a term in order to make the query and document match?
		/// 
		/// Phrase and proximity queries are more expensive than simple match queries. Whereas a match query just has to look up terms in the inverted index, 
		/// a match_phrase query has to calculate and compare the positions of multiple possibly repeated terms.
		/// The Lucene nightly benchmarks show that a simple term query is about 10 times as fast as a phrase query, 
		/// and about 20 times as fast as a proximity query (a phrase query with slop). And of course, this cost is paid at search time instead of at index time.
		/// 
		/// Usually the extra cost of phrase queries is not as scary as these numbers suggest. Really, the difference in performance is a testimony to just how fast a simple term query is. 
		/// Phrase queries on typical full-text data usually complete within a few milliseconds, and are perfectly usable in practice, even on a busy cluster.
		/// In certain pathological cases, phrase queries can be costly, but this is unusual. An example of a pathological case is DNA sequencing, 
		/// where there are many many identical terms repeated in many positions. Using higher slop values in this case results in a huge growth in the number of position calculations.
		/// 
		/// http://www.elasticsearch.org/guide/en/elasticsearch/guide/current/slop.html
		/// </summary>
		public int Slop
		{
			get { return _slop; }
			set
			{
				_slop = value;
				_slopSet = true;
			}
		}

		/// <summary>
		/// a max_expansions parameter that can control to how many prefixes the last term will be expanded. 
		/// It is highly recommended to set it to an acceptable value to control the execution time of the query.
		/// </summary>
		public int MaxExpansions
		{
			get { return _maxExpansions; }
			set
			{
				_maxExpansions = value;
				_maxExpansionsSet = true;
			}
		}

		//{
		// "query" : {
		//	  "match_phrase" : {
		//		"name" : {
		//			"query" : "group"
		//		}
		//	  }
		//  }
		//}
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("match_phrase_prefix");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteBasePropertiesJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("slop", _slop, elasticsearchCrudJsonWriter, _slopSet);
			JsonHelper.WriteValue("max_expansions", _maxExpansions, elasticsearchCrudJsonWriter, _maxExpansionsSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}