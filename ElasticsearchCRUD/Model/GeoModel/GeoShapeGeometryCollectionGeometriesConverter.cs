using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoShapeGeometryCollectionGeometriesConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			// it is the Geometries list
			if (objectType.IsGenericType)
			{
				var list = new List<object>();
				// It is a collection
				var ienumerable = (JArray)serializer.Deserialize(reader);
				foreach (var item in ienumerable)
				{
					var itemVal = item.ToString().Replace(Environment.NewLine, "").Replace(" ", "");
					var mygeoType = Regex.Match(itemVal, @"type\W+(?<type>\w+)\W+");
					var mygeoTypeString = mygeoType.Groups["type"].Value.ToLowerInvariant();
					switch (mygeoTypeString)
					{
						case DefaultGeoShapes.MultiLineString:
							list.Add(JsonConvert.DeserializeObject<GeoShapeMultiLineString>(itemVal));
							break;
						case DefaultGeoShapes.Point:
							list.Add(JsonConvert.DeserializeObject<GeoShapePoint>(itemVal));
							break;
						case DefaultGeoShapes.LineString:
							list.Add(JsonConvert.DeserializeObject<GeoShapeLineString>(itemVal));
							break;
						case DefaultGeoShapes.Circle:
							list.Add(JsonConvert.DeserializeObject<GeoShapeCircle>(itemVal));
							break;
						case DefaultGeoShapes.Envelope:
							list.Add(JsonConvert.DeserializeObject<GeoShapeEnvelope>(itemVal));
							break;
						case DefaultGeoShapes.MultiPoint:
							list.Add(JsonConvert.DeserializeObject<GeoShapeMultiPoint>(itemVal));
							break;
						case DefaultGeoShapes.MultiPolygon:
							list.Add(JsonConvert.DeserializeObject<GeoShapeMultiPolygon>(itemVal));
							break;
						case DefaultGeoShapes.Polygon:
							list.Add(JsonConvert.DeserializeObject<GeoShapePolygon>(itemVal));
							break;
					}
				}

				return list;
			}

			var jsonValueAsString = serializer.Deserialize(reader).ToString();
			var jsonValueAsStringTrimmed = jsonValueAsString.Replace(Environment.NewLine, "").Replace(" ", "");
			return JsonConvert.DeserializeObject<GeoShapeGeometryCollection>(jsonValueAsStringTrimmed);
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType.IsInterface)
			{
				return objectType == typeof(GeoShapeGeometryCollection);
			}
			return typeof(GeoShapeGeometryCollection).IsAssignableFrom(objectType);
		}
	}
}