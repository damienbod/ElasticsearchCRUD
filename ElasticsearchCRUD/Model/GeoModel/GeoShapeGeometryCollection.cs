using System.Collections.Generic;
using ElasticsearchCRUD.Utils;
using Newtonsoft.Json;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapeGeometryCollection : IGeoType
	{
		[JsonConverter(typeof(GeoShapeGeometryCollectionGeometriesConverter))]
		public List<object> Geometries { get; set; }

		public string Type { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.GeometryCollection, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geometries");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var item in Geometries)
			{
				(item as IGeoType).WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}