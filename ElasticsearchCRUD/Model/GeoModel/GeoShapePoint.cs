using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapePoint : GeoType
	{
		public Coordinate Coordinate { get; set; }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", DefaultGeoShapes.Point, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
			Coordinate.WriteJson(elasticsearchCrudJsonWriter);
		}
	}

	public abstract class GeoType
	{
		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}
