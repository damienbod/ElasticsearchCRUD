﻿using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// A filter that matches documents using the OR boolean operator on other filters. Can be placed within queries that accept a filter.
	/// </summary>
	public class OrFilter : IFilter
	{
		private List<IFilter> _or;
		private bool _orSet;
		private bool _cache;
		private bool _cacheSet;

		public List<IFilter> Or
		{
			get { return _or; }
			set
			{
				_or = value;
				_orSet = true;
			}
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("or");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteOrFilterList(elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void WriteOrFilterList(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_orSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filters");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var or in _or)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					or.WriteJson(elasticsearchCrudJsonWriter);
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}
	}
}