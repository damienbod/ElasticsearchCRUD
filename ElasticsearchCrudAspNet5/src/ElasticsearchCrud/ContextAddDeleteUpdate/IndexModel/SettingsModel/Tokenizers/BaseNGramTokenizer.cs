using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public abstract class BaseNGramTokenizer : AnalysisTokenizerBase
	{
		private int _minGram;
		private bool _minGramSet;
		private int _maxGram;
		private bool _maxGramSet;
		private List<TokenChar> _tokenChars;
		private bool _tokenCharsSet;

		/// <summary>
		/// min_gram Minimum size in codepoints of a single n-gram
		/// </summary>
		public int MinGram
		{
			get { return _minGram; }
			set
			{
				_minGram = value;
				_minGramSet = true;
			}
		}

		/// <summary>
		///  max_gram Maximum size in codepoints of a single n-gram
		/// </summary>
		public int MaxGram
		{
			get { return _maxGram; }
			set
			{
				_maxGram = value;
				_maxGramSet = true;
			}
		}

		/// <summary>
		/// token_chars Characters classes to keep in the tokens, Elasticsearch will split on characters that don’t belong to any of these classes.
		/// [] (Keep all characters)
		/// token_chars accepts the following character classes:
		///   letter for example a, b, ï or 京
		///   digit for example 3 or 7
		///   whitespace for example " " or "\n"
		///   punctuation for example ! or "
		///   symbol for example $ or √ 
		/// </summary>
		public List<TokenChar> TokenChars
		{
			get { return _tokenChars; }
			set
			{
				_tokenChars = value;
				_tokenCharsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("min_gram", _minGram, elasticsearchCrudJsonWriter, _minGramSet);
			JsonHelper.WriteValue("max_gram", _maxGram, elasticsearchCrudJsonWriter, _maxGramSet);
			WriteTokenCharValue("token_chars", _tokenChars, elasticsearchCrudJsonWriter, _tokenCharsSet);
		}

		public static void WriteTokenCharValue(string key, List<TokenChar> valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var obj in valueObj)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue(obj.ToString());
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}
	}
}