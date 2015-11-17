using System;
using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Sorting
{
	/// <summary>
	/// Allow to sort based on custom scripts
	/// 
	/// "sort" : {
	///	"_script" : {
	///		"script" : "doc['field_name'].value * factor",
	///		"type" : "number",
	///		"params" : {
	///			"factor" : 1.1
	///		},
	///		"order" : "asc"
	///	}
	///}
	/// </summary>
	public class SortScript : ISortHolder
	{
		private readonly string _script;
		private string _scriptType;
		private bool _scriptTypeSet;
		private List<ScriptParameter> _params;
		private bool _paramsSet;

		public SortScript(string script)
		{
			_script = script;
			Order = OrderEnum.asc;
		}

		public OrderEnum Order { get; set; }

		public string ScriptType
		{
			get { return _scriptType; }
			set
			{
				_scriptType = value;
				_scriptTypeSet = true;
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

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("sort");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("_script");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("script");
			elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
			JsonHelper.WriteValue("order", Order.ToString(), elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("type", _scriptType, elasticsearchCrudJsonWriter, _scriptTypeSet);
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

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}