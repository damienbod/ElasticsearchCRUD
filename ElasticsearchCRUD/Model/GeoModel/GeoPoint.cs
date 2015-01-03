using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.GeoModel
{
	public class GeoPoint : List<double>, GeoType
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
