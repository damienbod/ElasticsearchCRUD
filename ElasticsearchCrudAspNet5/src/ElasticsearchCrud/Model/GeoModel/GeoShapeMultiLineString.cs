using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapeMultiLineString : IGeoType
	{
		public List<List<GeoPoint>> Coordinates { get; set; }

		public string Type { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.MultiLineString, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var items in Coordinates)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
				foreach (var item in items)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}