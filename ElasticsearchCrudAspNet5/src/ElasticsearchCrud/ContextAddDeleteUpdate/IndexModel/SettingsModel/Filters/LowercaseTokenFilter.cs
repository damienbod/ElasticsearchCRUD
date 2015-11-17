using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class LowercaseTokenFilter : AnalysisFilterBase
	{
		private string _language;
		private bool _languageSet;

		/// <summary>
		/// A token filter of type lowercase that normalizes token text to lower case. 
		/// Lowercase token filter supports Greek, Irish, and Turkish lowercase token filters through the language parameter.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public LowercaseTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Lowercase;
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
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("language", _language, elasticsearchCrudJsonWriter, _languageSet);
		}
	}
}