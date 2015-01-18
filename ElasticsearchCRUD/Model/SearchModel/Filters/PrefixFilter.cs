using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class PrefixFilter :IFilter
	{
		private readonly string _field;
		private readonly object _prefix;
		private bool _cache;
		private bool _cacheSet;

		public PrefixFilter(string field, object prefix)
		{
			_field = field;
			_prefix = prefix;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("prefix");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue(_field, _prefix, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();	
		}
	}
}
