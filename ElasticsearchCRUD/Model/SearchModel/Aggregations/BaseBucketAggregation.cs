using System;
using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public abstract class BaseBucketAggregation : IAggs
	{
		private readonly string _type;
		private readonly string _name;
		private readonly string _field;

		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;
		private List<IAggs> _aggs;
		private bool _aggsSet;

		public BaseBucketAggregation(string type, string name, string field)
		{
			_type = type;
			_name = name;
			_field = field;
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

		public List<IAggs> Aggs
		{
			get { return _aggs; }
			set
			{
				_aggs = value;
				_aggsSet = true;
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

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		protected virtual void WriteJsonBase(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, Action<ElasticsearchCrudJsonWriter> writeFilterSpecific)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_type);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("field", _field, elasticsearchCrudJsonWriter);
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
			writeFilterSpecific.Invoke(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (_aggsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				foreach (var item in _aggs)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

		}
	}
}