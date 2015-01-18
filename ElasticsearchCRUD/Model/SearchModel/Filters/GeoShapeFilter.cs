using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class GeoShapeFilter : IFilter
	{
		private readonly string _field;
		private readonly IGeoType _geoType;
		private bool _cache;
		private bool _cacheSet;

		public GeoShapeFilter(string field, IGeoType geoType)
		{
			_field = field;
			_geoType = geoType;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geo_shape");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("shape");
		
			_geoType.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
