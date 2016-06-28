using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A range aggregation that is dedicated for date values. 
	/// The main difference between this aggregation and the normal range aggregation is that the from and to values can be expressed in Date Math expressions, 
	/// and it is also possible to specify a date format by which the from and to response fields will be returned. 
	/// Note that this aggregration includes the from value and excludes the to value for each range.
	/// 
	/// http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/search-aggregations-bucket-daterange-aggregation.html	
	/// </summary>
	public class DateRangeBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;
		private readonly string _format;
		private readonly List<RangeAggregationParameter<string>> _ranges;

		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;
		private bool _keyed;
		private bool _keyedSet;

		public DateRangeBucketAggregation(string name, string field, string format, List<RangeAggregationParameter<string>> ranges)
			: base("date_range", name)
		{
			_field = field;
			_format = format;
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
			JsonHelper.WriteValue("format", _format, elasticsearchCrudJsonWriter);
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