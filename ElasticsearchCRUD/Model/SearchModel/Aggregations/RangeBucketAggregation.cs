using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class RangeBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;
		private readonly List<RangeAggregationParameter> _ranges;

		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;
		private bool _keyed;
		private bool _keyedSet;

		public RangeBucketAggregation(string name, string field, List<RangeAggregationParameter> ranges ) : base("range", name)
		{
			_field = field;
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

		public abstract class RangeAggregationParameter
		{
			protected string KeyValue;
			protected bool KeySet;

			public string Key
			{
				get { return KeyValue; }
				set
				{
					KeyValue = value;
					KeySet = true;
				}
			}

			public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
		}

		public class ToRangeAggregationParameter : RangeAggregationParameter
		{
			private readonly object _value;

			public ToRangeAggregationParameter(object value)
			{
				_value = value;
			}

			public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				JsonHelper.WriteValue("key", KeyValue, elasticsearchCrudJsonWriter, KeySet);
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("to");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_value);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		public class FromRangeAggregationParameter : RangeAggregationParameter
		{
			private readonly object _value;

			public FromRangeAggregationParameter(object value)
			{
				_value = value;
			}

			public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				JsonHelper.WriteValue("key", KeyValue, elasticsearchCrudJsonWriter, KeySet);
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("from");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_value);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		public class ToFromRangeAggregationParameter : RangeAggregationParameter
		{
			private readonly object _to;
			private readonly object _from;
			
			public ToFromRangeAggregationParameter(object to, object from)
			{
				_to = to;
				_from = @from;
			}

			public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				JsonHelper.WriteValue("key", KeyValue, elasticsearchCrudJsonWriter, KeySet);
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("to");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_to);
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("from");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(_from);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}