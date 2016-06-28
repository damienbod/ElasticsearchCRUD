using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class DictionaryDecompounderTokenFilter : CompoundWordTokenFilter
	{
		public DictionaryDecompounderTokenFilter(string name) : base(name, DefaultTokenFilters.DictionaryDecompounder)
		{
		}
	}

	public class HyphenationDecompounderTokenFilter : CompoundWordTokenFilter
	{
		public HyphenationDecompounderTokenFilter(string name)
			: base(name, DefaultTokenFilters.HyphenationDecompounder)
		{
		}
	}

	public abstract class CompoundWordTokenFilter : AnalysisFilterBase
	{
		private bool _wordListSet;
		private List<string> _wordList;
		private string _hyphenationPatternsPath;
		private string _wordListPath;
		private bool _wordListPathSet;
		private bool _hyphenationPatternsPathSet;
		private bool _onlyLongestMatchSet;
		private bool _onlyLongestMatch;
		private bool _maxSubwordSizeSet;
		private int _maxSubwordSize;
		private bool _minSubwordSizeSet;
		private int _minSubwordSize;
		private bool _minWordSizeSet;
		private int _minWordSize;

		protected CompoundWordTokenFilter(string name, string type)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = type;
		}

		/// <summary>
		/// word_list
		/// A list of words to use.
		/// </summary>
		public List<string> WordList
		{
			get { return _wordList; }
			set
			{
				_wordList = value;
				_wordListSet = true;
			}
		}

		/// <summary>
		/// word_list_path
		/// A path (either relative to config location, or absolute) to a list of words.
		/// </summary>
		public string WordListPath
		{
			get { return _wordListPath; }
			set
			{
				_wordListPath = value;
				_wordListPathSet = true;
			}
		}

		/// <summary>
		/// hyphenation_patterns_path
		/// A path (either relative to config location, or absolute) to a FOP XML hyphenation pattern file. 
		/// (See http://offo.sourceforge.net/hyphenation/) Required for hyphenation_decompounder.
		/// </summary>
		public string HyphenationPatternsPath
		{
			get { return _hyphenationPatternsPath; }
			set
			{
				_hyphenationPatternsPath = value;
				_hyphenationPatternsPathSet = true;
			}
		}

		/// <summary>
		/// min_word_size
		/// Minimum word size(Integer). Defaults to 5.
		/// </summary>
		public int MinWordSize
		{
			get { return _minWordSize; }
			set
			{
				_minWordSize = value;
				_minWordSizeSet = true;
			}
		}

		/// <summary>
		/// min_subword_size
		/// Minimum subword size(Integer). Defaults to 2.
		/// </summary>
		public int MinSubwordSize
		{
			get { return _minSubwordSize; }
			set
			{
				_minSubwordSize = value;
				_minSubwordSizeSet = true;
			}
		}

		/// <summary>
		/// max_subword_size
		/// Maximum subword size(Integer). Defaults to 15.
		/// </summary>
		public int MaxSubwordSize
		{
			get { return _maxSubwordSize; }
			set
			{
				_maxSubwordSize = value;
				_maxSubwordSizeSet = true;
			}
		}

		/// <summary>
		/// only_longest_match
		/// Only matching the longest(Boolean). Defaults to false
		/// </summary>
		public bool OnlyLongestMatch
		{
			get { return _onlyLongestMatch; }
			set
			{
				_onlyLongestMatch = value;
				_onlyLongestMatchSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("word_list", _wordList, elasticsearchCrudJsonWriter, _wordListSet);
			JsonHelper.WriteValue("word_list_path", _wordListPath, elasticsearchCrudJsonWriter, _wordListPathSet);
			JsonHelper.WriteValue("hyphenation_patterns_path", _hyphenationPatternsPath, elasticsearchCrudJsonWriter, _hyphenationPatternsPathSet);
			JsonHelper.WriteValue("min_word_size", _minWordSize, elasticsearchCrudJsonWriter, _minWordSizeSet);
			JsonHelper.WriteValue("min_subword_size", _minSubwordSize, elasticsearchCrudJsonWriter, _minSubwordSizeSet);
			JsonHelper.WriteValue("max_subword_size", _maxSubwordSize, elasticsearchCrudJsonWriter, _maxSubwordSizeSet);
			JsonHelper.WriteValue("only_longest_match", _onlyLongestMatch, elasticsearchCrudJsonWriter, _onlyLongestMatchSet);
		}
	}
}