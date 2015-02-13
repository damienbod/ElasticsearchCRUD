using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	/// <summary>
	/// A rescore request is executed on each shard before it returns its results to be sorted by the node handling the overall search request.
	/// 
	/// Currently the rescore API has only one implementation: the query rescorer, which uses a query to tweak the scoring. 
	/// In the future, alternative rescorers may be made available, for example, a pair-wise rescorer.
	/// 
	/// Note: the rescore phase is not executed when search_type is set to scan or count.
	/// 
	/// The query rescorer executes a second query only on the Top-K results returned by the query and post_filter phases. 
	/// The number of docs which will be examined on each shard can be controlled by the window_size parameter, which defaults to from and size.
	/// 
	/// By default the scores from the original query and the rescore query are combined linearly to produce the final _score for each document. 
	/// The relative importance of the original query and of the rescore query can be controlled with the query_weight and rescore_query_weight respectively. Both default to 1.
	/// </summary>
	public class Rescore
	{
		private readonly IQuery _query;
		private readonly uint _windowSize;
		private double _queryWeight;
		private bool _queryWeightSet;
		private double _rescoreQueryWeight;
		private bool _rescoreQueryWeightSet;
		private ScoreModeRescore _scoreMode;
		private bool _scoreModeSet;

		public Rescore(IQuery query, uint windowSize)
		{
			_query = query;
			_windowSize = windowSize;
		}

		public double QueryWeight 
		{
			get { return _queryWeight; }
			set
			{
				_queryWeight = value;
				_queryWeightSet = true;
			}
		}

		/// <summary>
		/// rescore_query_weight
		/// </summary>
		public double RescoreQueryWeight 
		{
			get { return _rescoreQueryWeight; }
			set
			{
				_rescoreQueryWeight = value;
				_rescoreQueryWeightSet = true;
			}
		}

		/// <summary>
		/// score_mode
		/// </summary>
		public ScoreModeRescore ScoreMode 
		{
			get { return _scoreMode; }
			set
			{
				_scoreMode = value;
				_scoreModeSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("window_size", _windowSize, elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("rescore_query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_query.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("query_weight", _queryWeight, elasticsearchCrudJsonWriter, _queryWeightSet);
			JsonHelper.WriteValue("rescore_query_weight", _rescoreQueryWeight, elasticsearchCrudJsonWriter, _rescoreQueryWeightSet);
			JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticsearchCrudJsonWriter, _scoreModeSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum ScoreModeRescore
	{
		/// <summary>
		/// Average the original score and the rescore query score.
		/// </summary>
		avg,

		/// <summary>
		/// Take the max of original score and the rescore query score.
		/// </summary>
		max,

		/// <summary>
		/// Take the min of the original score and the rescore query score.
		/// </summary>
		min,

		/// <summary>
		/// Add the original score and the rescore query score. The default.
		/// </summary>
		total,

		/// <summary>
		/// Multiply the original score by the rescore query score. Useful for function query rescores.
		/// </summary>
		multiply
	}
}