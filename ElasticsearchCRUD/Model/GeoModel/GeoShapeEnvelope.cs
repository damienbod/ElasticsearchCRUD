using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapeEnvelope : GeoType
	{
		// TODO validate that at least two items exist
		public List<GeoPoint> Coordinates { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("type", DefaultGeoShapes.Envelope, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("envelope");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var item in Coordinates)
			{
				item.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}