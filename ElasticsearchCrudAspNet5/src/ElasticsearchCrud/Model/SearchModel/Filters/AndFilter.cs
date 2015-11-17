using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// A filter that matches documents using the AND boolean operator on other filters. Can be placed within queries that accept a filter.
	/// </summary>
	public class AndFilter : IFilter
	{
		private readonly List<IFilter> _and;
		private bool _cache;
		private bool _cacheSet;

		public AndFilter(List<IFilter> and)
		{
			_and = and;
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

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("and");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteAndFilterList(elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void WriteAndFilterList(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filters");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

			foreach (var and in _and)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				and.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
		}
	}
}
