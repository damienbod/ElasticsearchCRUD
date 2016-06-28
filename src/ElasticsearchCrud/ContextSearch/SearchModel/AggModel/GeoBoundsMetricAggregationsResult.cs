using ElasticsearchCRUD.Model.GeoModel;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class GeoBoundsMetricAggregationsResult : AggregationResult<GeoBoundsMetricAggregationsResult>
	{
		public GeoPoint BoundsTopLeft { get; set; }
		public GeoPoint BoundsBottomRight { get; set; }
	
		//  "bounds": {
		//		"top_left": {
		//			"lat": 80.45,
		//			"lon": -160.22
		//		},
		//		"bottom_right": {
		//			"lat": 40.65,
		//			"lon": 42.57
		//		}
		//	}

		public override GeoBoundsMetricAggregationsResult GetValueFromJToken(JToken result)
		{
			var topLeftLon = result["bounds"]["top_left"]["lon"].ToObject<double>();
			var topLeftLat = result["bounds"]["top_left"]["lat"].ToObject<double>();
			BoundsTopLeft = new GeoPoint(topLeftLon, topLeftLat);

			var bottomRightLon = result["bounds"]["bottom_right"]["lon"].ToObject<double>();
			var bottomRightLat = result["bounds"]["bottom_right"]["lat"].ToObject<double>();
			BoundsBottomRight = new GeoPoint(bottomRightLon, bottomRightLat);

			return this;
		}
	}
}