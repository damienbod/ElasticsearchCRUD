using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class CommonGramsFilter : AnalysisFilterBase
	{
		private bool _queryMode;
		private bool _queryModeSet;
		private bool _ignoreCaseSet;
		private bool _ignoreCase;
		private bool _commonWordsPathSet;
		private string _commonWordsPath;
		private bool _commonWordsSet;
		private List<string> _commonWords;

		/// <summary>
		/// Token filter that generates bigrams for frequently occuring terms. Single terms are still indexed. It can be used as an alternative to the
		/// Stop Token Filter when we don’t want to completely ignore common terms.
		/// For example, the text "the quick brown is a fox" will be tokenized as "the", "the_quick", "quick", "brown", "brown_is", "is_a", "a_fox", "fox". 
		/// Assuming "the", "is" and "a" are common words.
		/// 
		/// When query_mode is enabled, the token filter removes common words and single terms followed by a common word. 
		/// This parameter should be enabled in the search analyzer.
		/// For example, the query "the quick brown is a fox" will be tokenized as "the_quick", "quick", "brown_is", "is_a", "a_fox", "fox".
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public CommonGramsFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.CommonGrams;
		}

		/// <summary>
		/// common_words
		/// A list of common words to use.
		/// </summary>
		public List<string> CommonWords
		{
			get { return _commonWords; }
			set
			{
				_commonWords = value;
				_commonWordsSet = true;
			}
		}

		/// <summary>
		/// common_words_path
		/// A path (either relative to config location, or absolute) to a list of common words. Each word should be in its own "line" (separated by a line break). The file must be UTF-8 encoded.
		/// </summary>
		public string CommonWordsPath
		{
			get { return _commonWordsPath; }
			set
			{
				_commonWordsPath = value;
				_commonWordsPathSet = true;
			}
		}

		/// <summary>
		/// ignore_case
		/// If true, common words matching will be case insensitive (defaults to false).
		/// </summary>
		public bool IgnoreCase
		{
			get { return _ignoreCase; }
			set
			{
				_ignoreCase = value;
				_ignoreCaseSet = true;
			}
		}

		/// <summary>
		/// query_mode
		/// Generates bigrams then removes common words and single terms followed by a common word (defaults to false).
		/// </summary>
		public bool QueryMode
		{
			get { return _queryMode; }
			set
			{
				_queryMode = value;
				_queryModeSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("common_words", _commonWords, elasticsearchCrudJsonWriter, _commonWordsSet);
			JsonHelper.WriteValue("common_words_path", _commonWordsPath, elasticsearchCrudJsonWriter, _commonWordsPathSet);
			JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticsearchCrudJsonWriter, _ignoreCaseSet);
			JsonHelper.WriteValue("query_mode", _queryMode, elasticsearchCrudJsonWriter, _queryModeSet);
		}
	}
}