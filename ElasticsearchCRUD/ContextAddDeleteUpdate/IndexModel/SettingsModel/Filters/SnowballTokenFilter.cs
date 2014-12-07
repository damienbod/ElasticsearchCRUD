using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	/// <summary>
	/// A filter that stems words using a Snowball-generated stemmer. 
	/// The language parameter controls the stemmer with the following available values: 
	/// Armenian, Basque, Catalan, Danish, Dutch, English, Finnish, French, German, German2, Hungarian, 
	/// Italian, Kp, Lovins, Norwegian, Porter, Portuguese, Romanian, Russian, Spanish, Swedish, Turkish.
	/// </summary>
	public class SnowballTokenFilter : AnalysisFilterBase
	{
		private SnowballLanguage _language;
		private bool _languageSet;

		/// <summary>
		/// A filter that stems words using a Snowball-generated stemmer. 
		/// The language parameter controls the stemmer with the following available values: 
		/// Armenian, Basque, Catalan, Danish, Dutch, English, Finnish, French, German, German2, Hungarian, 
		/// Italian, Kp, Lovins, Norwegian, Porter, Portuguese, Romanian, Russian, Spanish, Swedish, Turkish.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public SnowballTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Snowball;
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
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("language", _language.ToString(), elasticsearchCrudJsonWriter, _languageSet);
		}
	}

	public enum SnowballLanguage
	{
		Armenian, Basque, Catalan, Danish, Dutch, English, Finnish, French, German, German2, Hungarian,
		Italian, Kp, Lovins, Norwegian, Porter, Portuguese, Romanian, Russian, Spanish, Swedish, Turkish
	}
}