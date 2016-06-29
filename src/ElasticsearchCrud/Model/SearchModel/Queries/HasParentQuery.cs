using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The has_parent query works the same as the has_parent filter, by automatically wrapping the filter with a constant_score 
	/// (when using the default score type). It has the same syntax as the has_parent filter.
	/// 
	/// The has_parent also has scoring support. The supported score types are score or none. 
	/// The default is none and this ignores the score from the parent document. 
	/// The score is in this case equal to the boost on the has_parent query (Defaults to 1). 
	/// If the score type is set to score, then the score of the matching parent document is aggregated into the child documents belonging to the matching parent document. 
	/// The score type can be specified with the score_mode field inside the has_parent query
	/// </summary>
	public class HasParentQuery : IQuery
	{
		private readonly string _type;
		private readonly IQuery _query;
		private ScoreModeHasParentQuery _scoreMode;
		private bool _scoreModeSet;
		private InnerHits _innerHits;
		private bool _innerHitsSet;

		public HasParentQuery(string type, IQuery query)
		{
			_type = type;
			_query = query;
		}

		/// <summary>
		/// score_mode
		/// score, none
		/// </summary>
		public ScoreModeHasParentQuery ScoreMode
		{
			get { return _scoreMode; }
			set
			{
				_scoreMode = value;
				_scoreModeSet = true;
			}
		}

		public InnerHits InnerHits
		{
			get { return _innerHits; }
			set
			{
				_innerHits = value;
				_innerHitsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("has_parent");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("parent_type", _type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticsearchCrudJsonWriter, _scoreModeSet);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_query.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (_innerHitsSet)
			{
				_innerHits.WriteJson(elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum ScoreModeHasParentQuery
	{
		score,
		none
	}
}