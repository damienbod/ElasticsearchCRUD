using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Nested query allows to query nested objects / docs (see nested mapping). 
	/// The query is executed against the nested objects / docs as if they were indexed as separate docs (they are, internally) 
	/// and resulting in the root parent doc (or parent nested mapping). 
	/// </summary>
	public class NestedQuery : IQuery
	{
		private readonly IQuery _query;
		private readonly string _path;
		private ScoreMode _scoreMode;
		private bool _scoreModeSet;
		private InnerHits _innerHits;
		private bool _innerHitsSet;

		public NestedQuery(IQuery query, string path)
		{
			_query = query;
			_path = path;
		}

		/// <summary>
		/// score_mode
		/// The score_mode allows to set how inner children matching affects scoring of parent. It defaults to avg, but can be sum, max and none.
		/// </summary>
		public ScoreMode ScoreMode
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("nested");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("path", _path, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_query.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticsearchCrudJsonWriter, _scoreModeSet);

			if (_innerHitsSet)
			{
				_innerHits.WriteJson(elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
