using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	/// <summary>
	/// The standard analyzer, which is the default analyzer used for full-text fields, is a good choice for most Western languages. It consists of the following:
	///
	///	The standard tokenizer, which splits the input text on word boundaries
	///	The standard token filter, which is intended to tidy up the tokens emitted by the tokenizer (but currently does nothing)
	///	The lowercase token filter, which converts all tokens into lowercase
	///	The stop token filter, which removes stopwords—common words that have little impact on search relevance, such as a, the, and, is. 
	///
	/// By default, the stopwords filter is disabled. You can enable it by creating a custom analyzer based on the standard analyzer and setting the stopwords parameter. 
	/// Either provide a list of stopwords or tell it to use a predefined stopwords list from a particular language.
	///
	/// "analyzer" : {
	///		"blocks_analyzer" : {
	///			"type" : "custom",
	///			"tokenizer" : "whitespace",
	///			"filter" : ["lowercase", "blocks_filter", "shingle"]
	///
	///          "pattern": "\\s+" TODO
	///          "char_filter": ["html_strip"] TODO
	///		}
	///	}
	/// </summary>
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
			if (_analyzerSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("analyzer");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				WriteValue("type", _type, elasticsearchCrudJsonWriter);
				WriteValue("tokenizer", _tokenizer, elasticsearchCrudJsonWriter, _tokenizerSet);
				WriteListValue("filter", _filter, elasticsearchCrudJsonWriter, _filterSet);

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
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
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var obj in valueObj)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}
	}
}