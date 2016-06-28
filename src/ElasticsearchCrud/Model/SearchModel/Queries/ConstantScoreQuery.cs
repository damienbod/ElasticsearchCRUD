using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// A query that wraps a filter or another query and simply returns a constant score equal to the query boost for every document in the filter. 
	/// Maps to Lucene ConstantScoreQuery.
	/// 
	/// The filter object can hold only filter elements, not queries. Filters can be much faster compared to queries since they don’t perform any scoring, especially when they are cached.
	/// </summary>
	public class ConstantScoreQuery : IQuery
	{
		private IQuery _query;
		private bool _querySet;
		private IFilter _filter;
		private bool _filterSet;
		private double _boost;
		private bool _boostSet;

		public ConstantScoreQuery(IQuery query)
		{
			Query = query;
		}

		public ConstantScoreQuery(IFilter filter)
		{
			Filter = filter;
		}

		/// <summary>
		/// positive
		/// </summary>
		public IQuery Query
		{
			get { return _query; }
			set
			{
				_query = value;
				_querySet = true;
			}
		}

		public IFilter Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				_filterSet = true;
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

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("constant_score");
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

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
