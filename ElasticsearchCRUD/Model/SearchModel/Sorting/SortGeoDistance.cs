using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Sorting
{
	public class SortGeoDistance : ISort
	{
		private readonly string _field;
		private SortModeGeo _mode;
		private bool _modeSet;
		private GeoPoint _geoPoint;
		private bool _geoPointSet;
		private List<GeoPoint> _geoPoints;
		private bool _geoPointsSet;

		public SortGeoDistance(string field, DistanceUnitEnum distanceUnit, GeoPoint geoPoint)
		{
			_field = field;
			Order = OrderEnum.asc;
			Unit = distanceUnit;
			GeoPoint = geoPoint;
		}

		public SortGeoDistance(string field, DistanceUnitEnum distanceUnit, List<GeoPoint> geoPoints)
		{
			_field = field;
			Order = OrderEnum.asc;
			Unit = distanceUnit;
			GeoPoints = geoPoints;
		}

		public OrderEnum Order { get; set; }

		/// <summary>
		/// mode
		/// Elasticsearch supports sorting by array or multi-valued fields. 
		/// The mode option controls what array value is picked for sorting the document it belongs to. The mode option can have the following values:
		/// SortMode enum: min, max, avg
		/// </summary>
		public SortModeGeo Mode
		{
			get { return _mode; }
			set
			{
				_mode = value;
				_modeSet = true;
			}
		}

		public GeoPoint GeoPoint
		{
			get { return _geoPoint; }
			set
			{
				_geoPoint = value;
				_geoPointSet = true;
			}
		}

		public List<GeoPoint> GeoPoints
		{
			get { return _geoPoints; }
			set
			{
				_geoPoints = value;
				_geoPointsSet = true;
			}
		}

		public DistanceUnitEnum Unit { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{		
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_geo_distance");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (_geoPointSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
				_geoPoint.WriteJson(elasticsearchCrudJsonWriter);
			}
			else if (_geoPointsSet)
			{		
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var item in _geoPoints)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
			JsonHelper.WriteValue("order", Order.ToString(), elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("mode", _mode.ToString(), elasticsearchCrudJsonWriter, _modeSet);
			JsonHelper.WriteValue("unit", Unit.ToString(), elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum SortModeGeo
	{
		/// <summary>
		/// Pick the lowest value.
		/// </summary>
		min,

		/// <summary>
		/// Pick the highest value.
		/// </summary>
		max,

		/// <summary>
		/// Use the average of all values as sort value. Only applicable for number based array fields. 
		/// </summary>
		avg

	}
}