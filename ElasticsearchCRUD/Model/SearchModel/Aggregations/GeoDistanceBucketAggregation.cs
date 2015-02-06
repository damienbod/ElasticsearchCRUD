using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class GeoDistanceBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;
		private readonly GeoPoint _origin;
		private readonly List<RangeAggregationParameter<uint>> _ranges;

		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;
		private bool _keyed;
		private bool _keyedSet;
		private DistanceUnitEnum _unit;
		private bool _unitSet;
		private DistanceType _distanceType;
		private bool _distanceTypeSet;

		public GeoDistanceBucketAggregation(string name, string field, GeoPoint origin, List<RangeAggregationParameter<uint>> ranges)
			: base("geo_distance", name)
		{
			_field = field;
			_origin = origin;
			_ranges = ranges;
		}

		/// <summary>
		/// If this value is set, the buckets are returned with id classes. 
		/// </summary>
		public bool Keyed
		{
			get { return _keyed; }
			set
			{
				_keyed = value;
				_keyedSet = true;
			}
		}

		public DistanceUnitEnum Unit
		{
			get { return _unit; }
			set
			{
				_unit = value;
				_unitSet = true;
			}
		}

		/// <summary>
		/// distance_type
		/// How to compute the distance. Can either be sloppy_arc (default), arc (slighly more precise but significantly slower) or plane 
		/// (faster, but inaccurate on long distances and close to the poles).
		/// </summary>
		public DistanceType DistanceType
		{
			get { return _distanceType; }
			set
			{
				_distanceType = value;
				_distanceTypeSet = true;
			}
		}

		public string Script
		{
			get { return _script; }
			set
			{
				_script = value;
				_scriptSet = true;
			}
		}

		public List<ScriptParameter> Params
		{
			get { return _params; }
			set
			{
				_params = value;
				_paramsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("field", _field, elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("origin");
			_origin.WriteJson(elasticsearchCrudJsonWriter);
			
			JsonHelper.WriteValue("unit", _unit.ToString(), elasticsearchCrudJsonWriter, _unitSet);			
			JsonHelper.WriteValue("distance_type", _distanceType.ToString(), elasticsearchCrudJsonWriter, _distanceTypeSet);
			JsonHelper.WriteValue("keyed", _keyed, elasticsearchCrudJsonWriter, _keyedSet);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("ranges");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();
			foreach (var rangeAggregationParameter in _ranges)
			{
				rangeAggregationParameter.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();

			if (_scriptSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("script");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
				if (_paramsSet)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("params");
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

					foreach (var item in _params)
					{
						elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
						elasticsearchCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
					}
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}
			}
		}
	}
}