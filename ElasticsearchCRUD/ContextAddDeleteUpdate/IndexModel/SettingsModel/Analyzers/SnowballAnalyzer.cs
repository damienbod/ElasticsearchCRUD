using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class SnowballAnalyzer: AnalyzerBase
	{
		private string _language;
		private bool _languageSet;

		public SnowballAnalyzer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultAnalyzers.Snowball;
		}


		public string Language
		{
			get { return _language; }
			set
			{
				_language = value;
				_languageSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteJsonBase(elasticsearchCrudJsonWriter, WriteSpecificJson);
		}

		private void WriteSpecificJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (AnalyzerSet)
			{
				JsonHelper.WriteValue("language", _language, elasticsearchCrudJsonWriter, _languageSet);	
			}
		}	
	}
}
