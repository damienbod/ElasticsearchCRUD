using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class GeohashCellFilter : IFilter
	{
		private readonly string _field;
		private readonly GeoPoint _location;
		private readonly int _precision;
		private readonly bool _neighbors;

		public GeohashCellFilter(string field, GeoPoint location, int precision, bool neighbors)
		{
			_field = field;
			_location = location;
			_precision = precision;
			_neighbors = neighbors;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geohash_cell");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			_location.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("precision", _precision, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("neighbors", _neighbors, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
