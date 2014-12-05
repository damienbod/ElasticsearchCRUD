using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class LanguageAnalyzer : BaseStopAnalyzer
	{
		private string _stopwordsPath;
		private bool _stopwordsPathSet;

		/// <summary>
		/// An analyzer of type stop that is built using a Lower Case Tokenizer, with Stop Token Filter.
		/// </summary>
		/// <param name="name">name of the analyzer</param>
		/// <param name="analyzer">required for language analyzers</param>
		public LanguageAnalyzer(string name, string analyzer)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type =  analyzer;		
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
			WriteCommonValues(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("stopwords_path", _stopwordsPath, elasticsearchCrudJsonWriter, _stopwordsPathSet);
		}
	}
}