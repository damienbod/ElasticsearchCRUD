using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapePoint : GeoType
	{
		public GeoPoint Coordinates { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.Point, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
			Coordinates.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public class GeoShapeCircle : GeoType
	{
		public GeoPoint Coordinates { get; set; }

		/// <summary>
		/// The inner radius field is required. If not specified, then the units of the radius will default to METERS.
		/// </summary>
		public string Radius { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.Circle, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
			Coordinates.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			JsonHelper.WriteValue("radius", Radius, elasticsearchCrudJsonWriter);
		}
	}
}
