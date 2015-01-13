using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class AndFilter : IFilter
	{
		private List<IFilter> _and;
		private bool _andSet;
		private bool _cache;
		private bool _cacheSet;

		public List<IFilter> And
		{
			get { return _and; }
			set
			{
				_and = value;
				_andSet = true;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("and");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteAndQueryList(elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void WriteAndQueryList(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_andSet)
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
}
