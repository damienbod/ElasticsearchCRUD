using System;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.GeoModel;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	public class SearchTest
	{
		public long Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchString(Analyzer = LanguageAnalyzers.German)]
		public string Details { get; set; }

		[ElasticsearchGeoShape]
		public GeoShapeCircle CircleTest { get; set; }

		[ElasticsearchGeoPoint(GeohashPrecision = "100km", GeohashPrefix = true, Geohash = true)]
		public GeoPoint Location { get; set; }

		public double Lift { get; set; }

		public DateTime DateOfDetails { get; set; }
	}
}