using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class SnowballAnalyzer : BaseStopAnalyzer
	{
		private SnowballLanguage _language;
		private bool _languageSet;

		/// <summary>
		/// An analyzer of type snowball that uses the standard tokenizer, with standard filter, lowercase filter, stop filter, and snowball filter.
		/// The Snowball Analyzer is a stemming analyzer from Lucene that is originally based on the snowball project from snowball.tartarus.org.
		/// </summary>
		/// <param name="name"></param>
		public SnowballAnalyzer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultAnalyzers.Snowball;
		}


		public SnowballLanguage Language
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
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteSpecificJson);
		}

		private void WriteSpecificJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (AnalyzerSet)
			{
				WriteCommonValues(elasticsearchCrudJsonWriter);
				JsonHelper.WriteValue("language", _language.ToString(), elasticsearchCrudJsonWriter, _languageSet);	
			}
		}	
	}
}
