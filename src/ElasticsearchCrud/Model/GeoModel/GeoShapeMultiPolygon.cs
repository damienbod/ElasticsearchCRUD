using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapeMultiPolygon : IGeoType
	{
		// TODO validate that first and the last items in each polygon are the same
		public List<List<List<GeoPoint>>> Coordinates { get; set; }

		public string Type { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.MultiPolygon, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var itemlists in Coordinates)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
				foreach (var items in itemlists)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
					foreach (var item in items)
					{
						item.WriteJson(elasticsearchCrudJsonWriter);
					}
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}