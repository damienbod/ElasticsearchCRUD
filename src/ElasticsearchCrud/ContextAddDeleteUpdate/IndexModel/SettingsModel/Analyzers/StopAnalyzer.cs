using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class StopAnalyzer : BaseStopAnalyzer
	{
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
			Type = DefaultAnalyzers.Stop;

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
