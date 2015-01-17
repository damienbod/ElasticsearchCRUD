using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// A filter allowing to filter hits based on a point location using a bounding box.
	/// </summary>
	public class GeoBoundingBoxFilter : IFilter
	{
		private readonly string _field;
		private readonly GeoPoint _topLeft;
		private readonly GeoPoint _bottomRight;
		private bool _cache;
		private bool _cacheSet;
		private GeoBoundingBoxFilterType _type;
		private bool _typeSet;

		public GeoBoundingBoxFilter(string field, GeoPoint topLeft, GeoPoint bottomRight)
		{
			_field = field;
			_topLeft = topLeft;
			_bottomRight = bottomRight;
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
		/// type
		/// The type of the bounding box execution by default is set to memory, which means in memory checks if the doc falls within the bounding box range. 
		/// In some cases, an indexed option will perform faster (but note that the geo_point type must have lat and lon indexed in this case). 
		/// Note, when using the indexed option, multi locations per document field are not supported.
		/// </summary>
		public GeoBoundingBoxFilterType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				_typeSet = true;
			}
		}
		 
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geo_bounding_box");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);
			JsonHelper.WriteValue("type", _type.ToString(), elasticsearchCrudJsonWriter, _typeSet);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("top_left");
			_topLeft.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("bottom_right");
			_bottomRight.WriteJson(elasticsearchCrudJsonWriter);


			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum GeoBoundingBoxFilterType
	{
		memory,
		indexed
	}
}

