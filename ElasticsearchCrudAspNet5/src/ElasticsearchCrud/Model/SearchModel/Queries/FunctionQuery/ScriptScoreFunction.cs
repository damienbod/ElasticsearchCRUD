using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class ScriptScoreFunction : BaseScoreFunction
	{
		private readonly string _script;
		private string _lang;
		private bool _langSet;
		private List<ScriptParameter> _params;
		private bool _paramsSet;

		public ScriptScoreFunction(string script)
		{
			_script = script;
		}

		public string Lang
		{
			get { return _lang; }
			set
			{
				_lang = value;
				_langSet = true;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("script_score");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("script", _script, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("lang", _lang, elasticsearchCrudJsonWriter, _langSet);
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
		}
	}
}
