using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// The indices query can be used when executed across multiple indices, 
	/// allowing to have a query that executes only when executed on an index that matches a specific list of indices, 
	/// and another filter that executes when it is executed on an index that does not match the listed indices.
	/// 
	/// The fields order is important: if the indices are provided before filter or no_match_query, 
	/// the related filters get parsed only against the indices that they are going to be executed on. 
	/// This is useful to avoid parsing queries when it is not necessary and prevent potential mapping errors.
	/// </summary>
	public class IndicesFilter : IFilter
	{
		private readonly List<string> _indices;
		private readonly IFilter _filter;
		private IFilter _noMatchFilter;
		private bool _noMatchFilterSet;
		private bool _noMatchFilterNone;
		private bool _noMatchFilterNoneSet;

		public IndicesFilter(List<string> indices, IFilter filter)
		{
			_indices = indices;
			_filter = filter;
		}

		/// <summary>
		/// no_match_filter
		/// </summary>
		public IFilter NoMatchFilter
		{
			get { return _noMatchFilter; }
			set
			{
				_noMatchFilter = value;
				_noMatchFilterSet = true;
			}
		}

		public bool NoMatchFilterNone
		{
			get { return _noMatchFilterNone; }
			set
			{
				_noMatchFilterNone = value;
				_noMatchFilterNoneSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("indices");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteListValue("indices", _indices, elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_filter.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (_noMatchFilterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("no_match_filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_noMatchFilter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			else if (!_noMatchFilterSet && _noMatchFilterNoneSet)
			{
				JsonHelper.WriteValue("no_match_filter", "none", elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}