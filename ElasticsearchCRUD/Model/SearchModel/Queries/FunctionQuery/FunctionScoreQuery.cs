using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	/// <summary>
	/// The function_score allows you to modify the score of documents that are retrieved by a query. 
	/// This can be useful if, for example, a score function is computationally expensive and it is sufficient to compute the score on a filtered set of documents.
	/// function_score provides the same functionality that custom_boost_factor, 
	/// custom_score and custom_filters_score provided but with additional capabilities such as distance and recency scoring
	/// </summary>
	public class FunctionScoreQuery : IQuery
	{
		private readonly IFilter _filter;
		private readonly IQuery _query;
		private readonly List<BaseScoreFunction> _functions;
		private readonly bool _querySet;
		private readonly bool _filterSet;
		private double _boost;
		private bool _boostSet;
		private FunctionScoreQueryScoreMode _scoreMode;
		private bool _scoreModeSet;
		private FunctionScoreQueryBoostMode _boostMode;
		private bool _boostModeSet;
		private double _maxBoost;
		private bool _maxBoostSet;

		public FunctionScoreQuery(IQuery query, List<BaseScoreFunction> functions)
		{
			_query = query;
			_functions = functions;
			_querySet = true;
		}

		public FunctionScoreQuery(IFilter filter, List<BaseScoreFunction> functions)
		{
			_filter = filter;
			_functions = functions;
			_filterSet = true;
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

		public double MaxBoost
		{
			get { return _maxBoost; }
			set
			{
				_maxBoost = value;
				_maxBoostSet = true;
			}
		}

		/// <summary>
		/// score_mode
		/// If no filter is given with a function this is equivalent to specifying "match_all": {}
		/// First, each document is scored by the defined functions. The parameter score_mode specifies how the computed scores are combined.
		/// 
		/// Because scores can be on different scales (for example, between 0 and 1 for decay functions but arbitrary for field_value_factor) 
		/// and also because sometimes a different impact of functions on the score is desirable, the score of each function can be adjusted with a user defined weight ( [1.4.0.Beta1] 
		/// Added in 1.4.0.Beta1.). The weight can be defined per function in the functions array (example above) and is multiplied with the score computed by the respective function. 
		/// If weight is given without any other function declaration, weight acts as a function that simply returns the weight.
		/// </summary>
		public FunctionScoreQueryScoreMode ScoreMode
		{
			get { return _scoreMode; }
			set
			{
				_scoreMode = value;
				_scoreModeSet = true;
			}
		}

		/// <summary>
		/// The new score can be restricted to not exceed a certain limit by setting the max_boost parameter. The default for max_boost is FLT_MAX.
		/// Finally, the newly computed score is combined with the score of the query. The parameter boost_mode defines how.
		/// </summary>
		public FunctionScoreQueryBoostMode BoostMode
		{
			get { return _boostMode; }
			set
			{
				_boostMode = value;
				_boostModeSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("function_score");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (_querySet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_query.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			if (_filterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_filter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("functions");			
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var function in _functions)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				function.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();

			JsonHelper.WriteValue("max_boost", _maxBoost, elasticsearchCrudJsonWriter, _maxBoostSet);
			JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticsearchCrudJsonWriter, _scoreModeSet);
			JsonHelper.WriteValue("boost_mode", _boostMode.ToString(), elasticsearchCrudJsonWriter, _boostModeSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
