using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public abstract class BaseStopAnalyzer : AnalyzerBase
	{
		private string _stopwords;
		private bool _stopwordsSet;
		private List<string> _stopwordsList;
		private bool _stopwordsListSet;

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

		protected void WriteCommonValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			//JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			if (_stopwordsListSet)
			{
				JsonHelper.WriteListValue("stopwords", _stopwordsList, elasticsearchCrudJsonWriter, _stopwordsListSet);
			}
			else
			{
				JsonHelper.WriteValue("stopwords", _stopwords, elasticsearchCrudJsonWriter, _stopwordsSet);
			}
		}

	}
}