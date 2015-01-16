using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Sorting
{
	public class GeoDistanceSort : ISort
	{
		private readonly string _field;
		private GeoSortMode _mode;
		private bool _modeSet;
		private SortMissing _missing;
		private bool _missingSet;
		private string _unmappedType;
		private bool _unmappedTypeSet;
		private GeoPoint _geoPoint;
		private bool _geoPointSet;
		private List<GeoPoint> _geoPoints;
		private bool _geoPointsSet;

		public GeoDistanceSort(string field, DistanceUnit distanceUnit)
		{
			_field = field;
			Order = OrderEnum.asc;
			Unit = distanceUnit;
		}

		public OrderEnum Order { get; set; }

		/// <summary>
		/// mode
		/// Elasticsearch supports sorting by array or multi-valued fields. 
		/// The mode option controls what array value is picked for sorting the document it belongs to. The mode option can have the following values:
		/// SortMode enum: min, max, sum, avg
		/// </summary>
		public GeoSortMode Mode
		{
			get { return _mode; }
			set
			{
				_mode = value;
				_modeSet = true;
			}
		}

		/// <summary>
		/// The missing parameter specifies how docs which are missing the field should be treated: 
		/// The missing value can be set to _last, _first, or a custom value (that will be used for missing docs as the sort value).
		/// </summary>
		public SortMissing Missing
		{
			get { return _missing; }
			set
			{
				_missing = value;
				_missingSet = true;
			}
		}

		/// <summary>
		/// unmapped_type
		/// By default, the search request will fail if there is no mapping associated with a field. The unmapped_type option allows to ignore fields that have no mapping and not sort by them. 
		/// The value of this parameter is used to determine what sort values to emit.
		/// 
		/// If any of the indices that are queried doesn’t have a mapping for price then Elasticsearch will handle it as if there was a mapping of type long, 
		/// with all documents in this index having no value for this field.
		/// </summary>
		public string UnmappedType
		{
			get { return _unmappedType; }
			set
			{
				_unmappedType = value;
				_unmappedTypeSet = true;
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

		public DistanceUnit Unit { get; set; }

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
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var item in _geoPoints)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
					item.WriteJson(elasticsearchCrudJsonWriter);
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
			JsonHelper.WriteValue("order", Order.ToString(), elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("mode", _mode.ToString(), elasticsearchCrudJsonWriter, _modeSet);
			JsonHelper.WriteValue("missing", _missing.ToString(), elasticsearchCrudJsonWriter, _missingSet);
			JsonHelper.WriteValue("unmapped_type", _unmappedType, elasticsearchCrudJsonWriter, _unmappedTypeSet);
			JsonHelper.WriteValue("unit", Unit.GetDistanceUnit(), elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum GeoSortMode
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