using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	/// <summary>
	/// The filtered query is used to combine another query with any filter. Filters are usually faster than queries because:
	///
    /// they don’t have to calculate the relevance _score for each document —  
    /// the answer is just a boolean “Yes, the document matches the filter” or “No, the document does not match the filter”.
    /// the results from most filters can be cached in memory, making subsequent executions faster. 
	///
    /// Tip Exclude as many document as you can with a filter, then query just the documents that remain.
    /// 
	/// If a query is not specified, it defaults to the match_all query. 
	/// This means that the filtered query can be used to wrap just a filter, so that it can be used wherever a query is expected.
	/// </summary>
	public class Filtered : IQuery
	{
		private Query _query;
		private readonly Filter _filter;
		private bool _querySet;
		private string _strategy;
		private bool _strategySet;

		public Filtered(Filter filter)
		{
			_filter = filter;
		}

		/// <summary>
		/// defaults to a match_all  query if is not set
		/// </summary>
		public Query Query
		{
			get { return _query; }
			set
			{
				_query = value;
				_querySet = true;
			}
		}

		/// <summary>
		/// strategy
		/// 
		/// The strategy parameter accepts the following options:
		///
		/// leap_frog_query_first      - Look for the first document matching the query, and then alternatively advance the query and the filter to find common matches.
		/// leap_frog_filter_first     - Look for the first document matching the filter, and then alternatively advance the query and the filter to find common matches.
		/// leap_frog                  - Same as leap_frog_query_first.
		/// query_first                - If the filter supports random access, then search for documents using the query, and then consult the filter to check whether there is a match. Otherwise fall back to leap_frog_query_first.
		/// random_access_${threshold} - If the filter supports random access and if there is at least one matching document among the first threshold ones, then apply the filter first. Otherwise fall back to leap_frog_query_first. ${threshold} must be greater than or equal to 1.
		/// random_access_always       - Apply the filter first if it supports random access. Otherwise fall back to leap_frog_query_first.
		///
		/// The default strategy is to use query_first on filters that are not advanceable such as geo filters and script filters, and random_access_100 on other filters.
		/// </summary>
		public string Strategy
		{
			get { return _strategy; }
			set
			{
				_strategy = value;
				_strategySet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filtered");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (_querySet)
			{
				_query.WriteJson(elasticsearchCrudJsonWriter);
			}

			_filter.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("strategy", _strategy, elasticsearchCrudJsonWriter, _strategySet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}