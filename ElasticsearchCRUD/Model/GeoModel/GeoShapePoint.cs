using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapePoint : GeoType
	{
		public GeoPoint Coordinate { get; set; }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", DefaultGeoShapes.Point, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
			Coordinate.WriteJson(elasticsearchCrudJsonWriter);
		}
	}
}
