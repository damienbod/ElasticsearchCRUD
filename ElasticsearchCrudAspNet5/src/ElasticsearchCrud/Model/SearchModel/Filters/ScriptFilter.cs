using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// Scripts are compiled and cached for faster execution. 
	/// If the same script can be used, just with different parameters provider, it is preferable to use the ability to pass parameters to the script itself
	/// </summary>
	public class ScriptFilter : IFilter
	{
		private readonly string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;

		public ScriptFilter(string script)
		{
			_script = script;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("script");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

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

			JsonHelper.WriteValue("lang", "groovy", elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
