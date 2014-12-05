using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class StopAnalyzer : AnalyzerBase
	{
		private string _stopwords;
		private bool _stopwordsSet;
		private List<string> _stopwordsList;
		private bool _stopwordsListSet;
		private string _stopwordsPath;
		private bool _stopwordsPathSet;

		/// <summary>
		/// An analyzer of type stop that is built using a Lower Case Tokenizer, with Stop Token Filter.
		/// </summary>
		/// <param name="name">name of the analyzer</param>
		public StopAnalyzer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultAnalyzers.Standard;
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

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("stopwords_path", _stopwordsPath, elasticsearchCrudJsonWriter, _stopwordsPathSet);

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
