using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class KeywordMarkerTokenFilter : AnalysisFilterBase
	{
		private string _keywordsPath;
		private bool _keywordsPathSet;
		private List<string> _keywords;
		private bool _keywordsSet;
		private bool _ignoreCase;
		private bool _ignoreCaseSet;

		/// <summary>
		/// Protects words from being modified by stemmers. Must be placed before any stemming filters.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public KeywordMarkerTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.KeywordMarker;
		}

		/// <summary>
		/// keywords
		/// A list of mapping rules to use.
		/// </summary>
		public List<string> Keywords
		{
			get { return _keywords; }
			set
			{
				_keywords = value;
				_keywordsSet = true;
			}
		}

		/// <summary>
		/// rules_path
		/// A path (either relative to config location, or absolute) to a list of words.
		/// </summary>
		public string KeywordsPath
		{
			get { return _keywordsPath; }
			set
			{
				_keywordsPath = value;
				_keywordsPathSet = true;
			}
		}

		/// <summary>
		/// ignore_case
		/// Set to true to lower case all words first. Defaults to false.
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

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("keywords", _keywords, elasticsearchCrudJsonWriter, _keywordsSet);
			JsonHelper.WriteValue("keywords_path", _keywordsPath, elasticsearchCrudJsonWriter, _keywordsPathSet);
			JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticsearchCrudJsonWriter, _ignoreCaseSet);
		}
	}
}