using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The top_children query runs the child query with an estimated hits size, and out of the hit docs, aggregates it into parent docs. 
	/// If there aren’t enough parent docs matching the requested from/size search request, then it is run again with a wider (more hits) search.
	/// 
	/// The top_children also provide scoring capabilities, with the ability to specify max, sum or avg as the score type.
	/// 
	/// One downside of using the top_children is that if there are more child docs matching the required hits when executing the child query, 
	/// then the total_hits result of the search response will be incorrect.
	/// 
	/// How many hits are asked for in the first child query run is controlled using the factor parameter (defaults to 5). For example, when asking for 10 parent docs (with from set to 0), 
	/// then the child query will execute with 50 hits expected. If not enough parents are found (in our example 10), 
	/// and there are still more child docs to query, then the child search hits are expanded by multiplying by the incremental_factor (defaults to 2).
	/// 
	/// The required parameters are the query and type (the child type to execute the query on).
	/// </summary>
	public class TopChildrenQuery : IQuery
	{
		private readonly string _type;
		private readonly IQuery _query;
		private Score _score;
		private bool _scoreSet;
		private uint _factor;
		private bool _factorSet;
		private uint _incrementalFactor;
		private bool _incrementalFactorSet;

		public TopChildrenQuery(string type, IQuery query)
		{
			_type = type;
			_query = query;
		}

		public Score Score
		{
			get { return _score; }
			set
			{
				_score = value;
				_scoreSet = true;
			}
		}

		/// <summary>
		/// _factor
		/// </summary>
		public uint Factor
		{
			get { return _factor; }
			set
			{
				_factor = value;
				_factorSet = true;
			}
		}

		/// <summary>
		/// incremental_factor
		/// </summary>
		public uint IncrementalFactor
		{
			get { return _incrementalFactor; }
			set
			{
				_incrementalFactor = value;
				_incrementalFactorSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("top_children");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", _type, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_query.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("factor", _factor, elasticsearchCrudJsonWriter, _factorSet);
			JsonHelper.WriteValue("incremental_factor", _incrementalFactor, elasticsearchCrudJsonWriter, _incrementalFactorSet);
			JsonHelper.WriteValue("score", _score.ToString(), elasticsearchCrudJsonWriter, _scoreSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum Score
	{
		avg,
		sum,
		max
	}
}
