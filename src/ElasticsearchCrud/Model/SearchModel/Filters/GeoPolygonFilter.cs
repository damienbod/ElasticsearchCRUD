using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class GeoPolygonFilter : IFilter
	{
		private readonly string _field;
		private readonly List<GeoPoint> _locations;

		public GeoPolygonFilter(string field, List<GeoPoint> locations)
		{
			if (locations.Count < 3)
			{
				throw new ElasticsearchCrudException("A Polygon has at least 3 points!");
			}
			_field = field;
			_locations = locations;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geo_polygon");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("points");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var item in _locations)
			{
				item.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}