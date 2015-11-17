using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.GeoModel
{
	/// <summary>
	/// Indexed Fields
	///
	/// The geo_point mapping will index a single field with the format of lat,lon. 
	/// The lat_lon option can be set to also index the .lat and .lon as numeric fields, and geohash can be set to true to also index .geohash value.
	///
	/// A good practice is to enable indexing lat_lon as well, since both the geo distance and bounding box filters can either be executed using in memory checks, 
	/// or using the indexed lat lon values, and it really depends on the data set which one performs better. Note though,
	///  that indexed lat lon only make sense when there is a single geo point value for the field, and not multi values.
	/// 
	/// Geohashes
	///
	/// Geohashes are a form of lat/lon encoding which divides the earth up into a grid. Each cell in this grid is represented by a geohash string. 
	/// Each cell in turn can be further subdivided into smaller cells which are represented by a longer string. So the longer the geohash, the smaller (and thus more accurate) the cell is.
	///
	/// Because geohashes are just strings, they can be stored in an inverted index like any other string, which makes querying them very efficient.
	///
	/// If you enable the geohash option, a geohash “sub-field” will be indexed as, eg pin.geohash. The length of the geohash is controlled by the geohash_precision parameter, 
	/// which can either be set to an absolute length (eg 12, the default) or to a distance (eg 1km).
	///
	/// More usefully, set the geohash_prefix option to true to not only index the geohash value, but all the enclosing cells as well. 
	/// For instance, a geohash of u30 will be indexed as [u,u3,u30]. This option can be used by the Geohash Cell Filter to find geopoints within a particular cell very efficiently.
	/// </summary>
	public class GeoPoint : List<double>, IGeoType
	{
		public GeoPoint()
		{
		}

		public GeoPoint(double longitude, double latitude)
		{
			_longitude = longitude;
			_latitude = latitude;
			base.Add(_longitude);
			base.Add(_latitude);
		}

		private double _latitude;
		private double _longitude;

		/// <summary>
		/// -90 to +90 for Latitude
		/// </summary>
		public double Latitude
		{
			get { return _latitude; }
			set
			{
				GuardLatitude(value);
				_latitude = value;
				this[1] = _latitude;
			}
		}

		/// <summary>
		/// -180 to +180 for Longitude
		/// </summary>
		public double Longitude
		{
			get { return _longitude; }
			set
			{
				GuardLongitude(value);
				_longitude = value;
				this[0] = _latitude;
			}
		}

		/// <summary>
		/// -90 to +90 for Latitude
		/// </summary>
		private void GuardLatitude(double latitude)
		{
			if (latitude >= -90 && latitude <= 90)
			{
				throw new ElasticsearchCrudException("latitude outside possible range:" + latitude);
			}
		}

		/// <summary>
		/// -180 to +180 for Longitude
		/// </summary>
		private void GuardLongitude(double longitude)
		{
			if (longitude >= -180 && longitude <= 180)
			{
				throw new ElasticsearchCrudException("longitude outside possible range:" + longitude);
			}
		}

		public string Type { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_longitude);
			elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_latitude);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
		}

		/// <summary>
		/// This method cannot be used. Set the Longitude or the Latitude properties
		/// </summary>
		public new void Add(double point)
		{
			throw new ElasticsearchCrudException("GeoPoint. This method cannot be used. Set the Longitude or the Latitude properties");
		}
	}
}
