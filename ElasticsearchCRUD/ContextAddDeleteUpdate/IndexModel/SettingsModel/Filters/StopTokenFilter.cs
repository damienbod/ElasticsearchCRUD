using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class StopTokenFilter : AnalysisFilterBase
	{
		private string _stopwords;
		private bool _stopwordsSet;
		private List<string> _stopwordsList;
		private bool _stopwordsListSet;
		private string _stopwordsPath;
		private bool _stopwordsPathSet;
		private bool _ignoreCase;
		private bool _ignoreCaseSet;
		private bool _removeTrailing;
		private bool _removeTrailingSet;

		public StopTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Stop;
		}

		/// <summary>
		/// A list of stopwords to initialize the stop filter with. Defaults to the english stop words.
		/// Use stopwords: _none_ to explicitly specify an empty stopword list.
		/// </summary>
		public string Stopwords
		{
			get { return _stopwords; }
			set
			{
				_stopwords = value;
				_stopwordsSet = true;
			}
		}

		public List<string> StopwordsList
		{
			get { return _stopwordsList; }
			set
			{
				_stopwordsList = value;
				_stopwordsListSet = true;
			}
		}

		/// <summary>
		/// stopwords_path
		/// A path (either relative to config location, or absolute) to a stopwords file configuration.
		/// </summary>
		public string StopwordsPath
		{
			get { return _stopwordsPath; }
			set
			{
				_stopwordsPath = value;
				_stopwordsPathSet = true;
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

		/// <summary>
		/// remove_trailing
		/// Set to false in order to not ignore the last term of a search if it is a stop word. 
		/// This is very useful for the completion suggester as a query like green a can be 
		/// extended to green apple even though you remove stop words in general. Defaults to true. 
		/// </summary>
		public bool RemoveTrailing
		{
			get { return _removeTrailing; }
			set
			{
				_removeTrailing = value;
				_removeTrailingSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			if (_stopwordsListSet)
			{
				JsonHelper.WriteListValue("stopwords", _stopwordsList, elasticsearchCrudJsonWriter, _stopwordsListSet);
			}
			else
			{
				JsonHelper.WriteValue("stopwords", _stopwords, elasticsearchCrudJsonWriter, _stopwordsSet);
			}
			JsonHelper.WriteValue("stopwords_path", _stopwordsPath, elasticsearchCrudJsonWriter, _stopwordsPathSet);
			JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticsearchCrudJsonWriter, _ignoreCaseSet);
			JsonHelper.WriteValue("remove_trailing", _removeTrailing, elasticsearchCrudJsonWriter, _removeTrailingSet);
		}
	}
}