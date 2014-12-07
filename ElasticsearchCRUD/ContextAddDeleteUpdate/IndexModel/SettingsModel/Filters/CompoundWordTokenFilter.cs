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

//word_list_path
//A path (either relative to config location, or absolute) to a list of words.

//hyphenation_patterns_path
//A path (either relative to config location, or absolute) to a FOP XML hyphenation pattern file. (See http://offo.sourceforge.net/hyphenation/) Required for hyphenation_decompounder.

//min_word_size
//Minimum word size(Integer). Defaults to 5.

//min_subword_size
//Minimum subword size(Integer). Defaults to 2.

//max_subword_size
//Maximum subword size(Integer). Defaults to 15.

//only_longest_match
//Only matching the longest(Boolean). Defaults to false

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("word_list", _wordList, elasticsearchCrudJsonWriter, _wordListSet);

		}
	}
}