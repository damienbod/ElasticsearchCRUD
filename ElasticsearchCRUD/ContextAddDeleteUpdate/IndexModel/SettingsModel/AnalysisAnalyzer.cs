using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	//	"analyzer" : {
	//		"blocks_analyzer" : {
	//			"type" : "custom",
	//			"tokenizer" : "whitespace",
	//			"filter" : ["lowercase", "blocks_filter", "shingle"]
	//		}
	//	}
	public class AnalysisAnalyzer
	{
		private bool _analyzerSet;
		private string _name;
		private string _type;
		private string _tokenizer;
		private bool _tokenizerSet;
		private List<string> _filter;
		private bool _filterSet
			;

		public void SetAnalyzer(string name, string type)
		{
			_analyzerSet = true;
			_name = name.ToLower();
			_type = type;
		}

		public string Tokenizer
		{
			get { return _tokenizer; }
			set
			{
				_tokenizer = value;
				_tokenizerSet = true;
			}
		}

		public List<string> Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				_filterSet = true;
			}
		}

		public virtual void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteValue("tokenizer", _tokenizer, elasticsearchCrudJsonWriter, _tokenizerSet);
		}

		private void WriteValue(string key, object valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
			}
		}

		private void WriteListValue(string key, List<string> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				foreach (var obj in valueObj)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj);
				}

			}
		}
	}
}