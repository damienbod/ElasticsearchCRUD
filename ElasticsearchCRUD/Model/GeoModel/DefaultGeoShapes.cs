namespace ElasticsearchCRUD.Model.GeoModel
{
	public class DefaultGeoShapes
	{
		/// <summary>
		/// A single geographic coordinate.
		/// </summary>
		public const string Point = "point";

		/// <summary>
		/// An arbitrary line given two or more points.
		/// </summary>
		public const string LineString = "linestring";
	
		/// <summary>
		/// A closed polygon whose first and last point must match, thus requiring n + 1 vertices to create an n-sided polygon and a minimum of 4 vertices.
		/// </summary>
		public const string Polygon = "polygon";

		/// <summary>
		/// An array of unconnected, but likely related points.
		/// </summary>
		public const string MultiPoint = "multipoint";

		/// <summary>
		/// An array of separate linestrings.
		/// </summary>
		public const string MultiLineString = "multilinestring";

		/// <summary>
		/// An array of separate polygons.
		/// </summary>
		public const string MultiPolygon = "multipolygon";

		/// <summary>
		/// A GeoJSON shape similar to the multi* shapes except that multiple types can coexist (e.g., a Point and a LineString).
		/// </summary>
		public const string GeometryCollection = "geometrycollection";

		/// <summary>
		/// A bounding rectangle, or envelope, specified by specifying only the top left and bottom right points.
		/// </summary>
		public const string Envelope = "envelope";

		/// <summary>
		/// A circle specified by a center point and radius with units, which default to METERS.
		/// </summary>
		public const string Circle = "circle";
	}
}
