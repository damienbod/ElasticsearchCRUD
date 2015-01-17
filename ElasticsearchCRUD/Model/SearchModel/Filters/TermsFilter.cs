using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class TermsFilter : IFilter
	{
		private readonly string _term;
		private readonly List<object> _termValues;
		private bool _cache;
		private bool _cacheSet;
		private ExecutionMode _execution;
		private bool _executionSet;

		public TermsFilter(string term, List<object> termValues)
		{
			_term = term;
			_termValues = termValues;
		}

		
		public bool Cache
		{
			get { return _cache; }
			set
			{
				_cache = value;
				_cacheSet = true;
			}
		}

		/// <summary>
		/// The way terms filter executes is by iterating over the terms provided and finding matches docs (loading into a bitset) and caching it. 
		/// Sometimes, we want a different execution model that can still be achieved by building more complex queries in the DSL, 
		/// but we can support them in the more compact model that terms filter provides.
		/// </summary>
		public ExecutionMode Execution
		{
			get { return _execution; }
			set
			{
				_execution = value;
				_executionSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("terms");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteListValue(_term, _termValues, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);
			JsonHelper.WriteValue("execution", _execution.ToString(), elasticsearchCrudJsonWriter, _executionSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum ExecutionMode
	{
		/// <summary>
		/// The default. Works as today. Iterates over all the terms, building a bit set matching it, and filtering. The total filter is cached.
		/// </summary>
		plain,
		
		/// <summary>
		/// Generates a terms filters that uses the fielddata cache to compare terms. 
		/// This execution mode is great to use when filtering on a field that is already loaded into the fielddata cache from faceting, sorting, 
		/// or index warmers. When filtering on a large number of terms, this execution can be considerably faster than the other modes. 
		/// The total filter is not cached unless explicitly configured to do so.
		/// </summary>
		fielddata,

		/// <summary>
		/// Generates a term filter (which is cached) for each term, and wraps those in a bool filter. 
		/// The bool filter itself is not cached as it can operate very quickly on the cached term filters.
		/// </summary>
		@bool,

		/// <summary>
		/// Generates a term filter (which is cached) for each term, and wraps those in an and filter. The and filter itself is not cached.
		/// </summary>
		and,
		
		/// <summary>
		/// Generates a term filter (which is cached) for each term, and wraps those in an or filter. The or filter itself is not cached. Generally, 
		/// the bool execution mode should be preferred. 
		/// </summary>
		or
	}
}
