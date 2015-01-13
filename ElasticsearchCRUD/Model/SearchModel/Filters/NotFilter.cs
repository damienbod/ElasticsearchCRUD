using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class NotFilter : IFilter
	{
		private IFilter _not;
		private bool _notSet;
		private bool _cache;
		private bool _cacheSet;

		public NotFilter(IFilter not)
		{
			Not = not;
		}

		public IFilter Not
		{
			get { return _not; }
			set
			{
				_not = value;
				_notSet = true;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("not");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteNotFilter(elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void WriteNotFilter(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_notSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_not.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}