using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class NestedFilter : IFilter
	{
		private readonly IFilter _filter;
		private readonly string _path;
		private bool _cache;
		private bool _cacheSet;
		private bool _join;
		private bool _joinSet;

		public NestedFilter(IFilter filter, string path)
		{
			_filter = filter;
			_path = path;
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

		public bool Join
		{
			get { return _join; }
			set
			{
				_join = value;
				_joinSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("nested");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("path", _path, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_filter.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);
			JsonHelper.WriteValue("join", _join, elasticsearchCrudJsonWriter, _joinSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
