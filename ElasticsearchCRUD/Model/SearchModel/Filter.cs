using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	/// <summary>
	/// As a general rule, filters should be used instead of queries:
	///
    /// - for binary yes/no searches
    /// - for queries on exact values 
    /// 
    /// Filters and Caching
	///
	/// Filters can be a great candidate for caching. Caching the result of a filter does not require a lot of memory, 
	/// and will cause other queries executing against the same filter (same parameters) to be blazingly fast.
	/// 
	/// Some filters already produce a result that is easily cacheable, and the difference between caching and not caching them is the act of placing the result in the cache or not. 
	/// These filters, which include the term, terms, prefix, and range filters, are by default cached and
	/// are recommended to use (compared to the equivalent query version) when the same filter (same parameters) will be used across multiple different queries 
	/// (for example, a range filter with age higher than 10).
	/// 
	/// Other filters, usually already working with the field data loaded into memory, are not cached by default. Those filters are already very fast, 
	/// and the process of caching them requires extra processing in order to allow the filter result to be used with different queries than the one executed. 
	/// These filters, including the geo, and script filters are not cached by default.
	/// 
	/// The last type of filters are those working with other filters. The and, not and or filters are not cached as they basically just manipulate the internal filters.
	/// 
	/// All filters allow to set _cache element on them to explicitly control caching. They also allow to set _cache_key which will be used as the caching key for that filter. 
	/// This can be handy when using very large filters (like a terms filter with many elements in it).
	/// </summary>
	public class Filter : IFilterHolder
	{
		private readonly IFilter _filter;
		private string _name;
		private bool _nameSet;

		public Filter(IFilter filter)
		{
			_filter = filter;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				_nameSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_filter.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("_name", _name,elasticsearchCrudJsonWriter,_nameSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}