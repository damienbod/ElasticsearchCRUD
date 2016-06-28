using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// map_script
	/// Executed once per document collected. This is the only required script. If no combine_script is specified, the resulting state needs to be stored in an object named _agg.
	/// </summary>
	public class ScriptedMetricAggregation : IAggs
	{
		private readonly string _name;
		private readonly string _mapScript;
		private ParamsForScript _params;
		private bool _paramsSet;
		private string _initScript;
		private bool _initScriptSet;
		private string _combineScript;
		private bool _combineScriptSet;
		private string _reduceScript;
		private bool _reduceScriptSet;
		private ParamsForScript _reduceParams;
		private bool _reduceParamsSet;
		private string _lang;
		private bool _langSet;
		private string _initScriptFile;
		private bool _initScriptFileSet;
		private string _initScriptId;
		private bool _initScriptIdSet;
		private string _mapScriptFile;
		private bool _mapScriptFileSet;
		private string _mapScriptId;
		private bool _mapScriptIdSet;
		private string _combineScriptFile;
		private bool _combineScriptFileSet;
		private string _combineScriptId;
		private bool _combineScriptIdSet;
		private string _reduceScriptFile;
		private bool _reduceScriptFileSet;
		private string _reduceScriptId;
		private bool _reduceScriptIdSet;

		public ScriptedMetricAggregation(string name, string mapScript)
		{
			_name = name;
			_mapScript = mapScript;
		}

		public ParamsForScript Params
		{
			get { return _params; }
			set
			{
				_params = value;
				_paramsSet = true;
			}
		}

		/// <summary>
		/// init_script
		/// Executed prior to any collection of documents. Allows the aggregation to set up any initial state.
		/// </summary>
		public string InitScript
		{
			get { return _initScript; }
			set
			{
				_initScript = value;
				_initScriptSet = true;
			}
		}

		/// <summary>
		/// combine_script
		/// Executed once on each shard after document collection is complete. Allows the aggregation to consolidate the state returned from each shard.
		///  If a combine_script is not provided the combine phase will return the aggregation variable.
		/// </summary>
		public string CombineScript
		{
			get { return _combineScript; }
			set
			{
				_combineScript = value;
				_combineScriptSet = true;
			}
		}

		/// <summary>
		/// reduce_script
		/// Executed once on the coordinating node after all shards have returned their results. The script is provided with access to a variable _aggs which is an array of the result of the combine_script on each shard. 
		/// If a reduce_script is not provided the reduce phase will return the _aggs variable.
		/// </summary>
		public string ReduceScript
		{
			get { return _reduceScript; }
			set
			{
				_reduceScript = value;
				_reduceScriptSet = true;
			}
		}

		/// <summary>
		/// reduce_params
		/// Optional. An object whose contents will be passed as variables to the reduce_script. 
		/// This can be useful to allow the user to control the behavior of the reduce phase. If this is not specified the variable will be undefined in the reduce_script execution.
		/// </summary>
		public ParamsForScript ReduceParams
		{
			get { return _reduceParams; }
			set
			{
				_reduceParams = value;
				_reduceParamsSet = true;
			}
		}

		/// <summary>
		/// lang
		/// Optional. The script language used for the scripts. If this is not specified the default scripting language is used.
		/// </summary>
		public string Lang
		{
			get { return _lang; }
			set
			{
				_lang = value;
				_langSet = true;
			}
		}

		/// <summary>
		/// init_script_file
		/// Optional. Can be used in place of the init_script parameter to provide the script using in a file.
		/// </summary>
		public string InitScriptFile
		{
			get { return _initScriptFile; }
			set
			{
				_initScriptFile = value;
				_initScriptFileSet = true;
			}
		}

		/// <summary>
		/// init_script_id
		/// Optional. Can be used in place of the init_script parameter to provide the script using an indexed script.
		/// </summary>
		public string InitScriptId
		{
			get { return _initScriptId; }
			set
			{
				_initScriptId = value;
				_initScriptIdSet = true;
			}
		}

		/// <summary>
		/// map_script_file
		/// Optional. Can be used in place of the map_script parameter to provide the script using in a file.
		/// </summary>
		public string MapScriptFile
		{
			get { return _mapScriptFile; }
			set
			{
				_mapScriptFile = value;
				_mapScriptFileSet = true;
			}
		}

		/// <summary>
		/// map_script_id
		/// Optional. Can be used in place of the map_script parameter to provide the script using an indexed script.
		/// </summary>
		public string MapScriptId
		{
			get { return _mapScriptId; }
			set
			{
				_mapScriptId = value;
				_mapScriptIdSet = true;
			}
		}

		/// <summary>
		/// combine_script_file
		/// Optional. Can be used in place of the combine_script parameter to provide the script using in a file.
		/// </summary>
		public string CombineScriptFile
		{
			get { return _combineScriptFile; }
			set
			{
				_combineScriptFile = value;
				_combineScriptFileSet = true;
			}
		}

		/// <summary>
		/// combine_script_id
		/// Optional. Can be used in place of the combine_script parameter to provide the script using an indexed script.
		/// </summary>
		public string CombineScriptId
		{
			get { return _combineScriptId; }
			set
			{
				_combineScriptId = value;
				_combineScriptIdSet = true;
			}
		}

		/// <summary>
		/// reduce_script_file
		/// Optional. Can be used in place of the reduce_script parameter to provide the script using in a file.
		/// </summary>
		public string ReduceScriptFile
		{
			get { return _reduceScriptFile; }
			set
			{
				_reduceScriptFile = value;
				_reduceScriptFileSet = true;
			}
		}

		/// <summary>
		/// reduce_script_id
		/// Optional. Can be used in place of the reduce_script parameter to provide the script using an indexed script. 
		/// </summary>
		public string ReduceScriptId
		{
			get { return _reduceScriptId; }
			set
			{
				_reduceScriptId = value;
				_reduceScriptIdSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("scripted_metric");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("map_script");
			elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _mapScript + "\"");

			if (_initScriptSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("init_script");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _initScript + "\"");
			}

			if (_combineScriptSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("combine_script");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _combineScript + "\"");
			}

			if (_reduceScriptSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("reduce_script");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _reduceScript + "\"");
			}

			if (_paramsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("params");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_params.TransactionName);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				foreach (var item in _params.Params)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			if (_reduceParamsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("reduce_params");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_reduceParams.TransactionName);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				foreach (var item in _reduceParams.Params)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			JsonHelper.WriteValue("lang", _lang, elasticsearchCrudJsonWriter, _langSet);
			JsonHelper.WriteValue("init_script_file", _initScriptFile, elasticsearchCrudJsonWriter, _initScriptFileSet);
			JsonHelper.WriteValue("init_script_id", _initScriptId, elasticsearchCrudJsonWriter, _initScriptIdSet);
			JsonHelper.WriteValue("map_script_file", _mapScriptFile, elasticsearchCrudJsonWriter, _mapScriptFileSet);
			JsonHelper.WriteValue("map_script_id", _mapScriptId, elasticsearchCrudJsonWriter, _mapScriptIdSet);
			JsonHelper.WriteValue("combine_script_file", _combineScriptFile, elasticsearchCrudJsonWriter, _combineScriptFileSet);
			JsonHelper.WriteValue("combine_script_id", _combineScriptId, elasticsearchCrudJsonWriter, _combineScriptIdSet);
			JsonHelper.WriteValue("reduce_script_file", _reduceScriptFile, elasticsearchCrudJsonWriter, _reduceScriptFileSet);
			JsonHelper.WriteValue("reduce_script_id", _reduceScriptId, elasticsearchCrudJsonWriter, _reduceScriptIdSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public class ParamsForScript
	{
		public string TransactionName;

		public ParamsForScript(string transactionName)
		{
			TransactionName = transactionName;
		}

		public List<ScriptParameter> Params { get; set; }
	}
}
