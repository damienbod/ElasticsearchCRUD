using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapeGeometryCollection : GeoType
	{
		public List<GeoType> Geometries { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.GeometryCollection, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geometries");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var item in Geometries)
			{
				item.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}