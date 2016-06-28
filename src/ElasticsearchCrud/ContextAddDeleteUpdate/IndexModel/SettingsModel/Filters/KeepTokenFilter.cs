using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class KeepTokenFilter : AnalysisFilterBase
	{
		private bool _keepWordsCaseSet;
		private List<string> _keepWords;
		private bool _keepWordsSet;
		private string _keepWordsPath;
		private bool _keepWordsCase;
		private bool _keepWordsPathSet;

		/// <summary>
		/// A token filter of type keep that only keeps tokens with text contained in a predefined set of words. 
		/// The set of words can be defined in the settings or loaded from a text file containing one word per line.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public KeepTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Keep;
		}

		/// <summary>
		/// keep_words
		/// a list of words to keep
		/// </summary>
		public List<string> KeepWords
		{
			get { return _keepWords; }
			set
			{
				_keepWords = value;
				_keepWordsSet = true;
			}
		}

		/// <summary>
		/// keep_words_path
		/// a path to a words file
		/// </summary>
		public string KeepWordsPath
		{
			get { return _keepWordsPath; }
			set
			{
				_keepWordsPath = value;
				_keepWordsPathSet = true;
			}
		}

		/// <summary>
		/// keep_words_case
		/// a boolean indicating whether to lower case the words (defaults to false) 
		/// </summary>
		public bool KeepWordsCase
		{
			get { return _keepWordsCase; }
			set
			{
				_keepWordsCase = value;
				_keepWordsCaseSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("keep_words", _keepWords, elasticsearchCrudJsonWriter, _keepWordsSet);
			JsonHelper.WriteValue("keep_words_path", _keepWordsPath, elasticsearchCrudJsonWriter, _keepWordsPathSet);
			JsonHelper.WriteValue("keep_words_case", _keepWordsCase, elasticsearchCrudJsonWriter, _keepWordsCaseSet);
		}
	}
}