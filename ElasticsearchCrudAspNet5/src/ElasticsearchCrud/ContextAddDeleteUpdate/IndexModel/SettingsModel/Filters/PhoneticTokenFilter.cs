using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class PhoneticTokenFilter : AnalysisFilterBase
	{
		private PhoneticEncoder _encoder;
		private bool _encoderSet;
		private bool _replace;
		private bool _replaceSet;

		/// <summary>
		/// https://github.com/elasticsearch/elasticsearch-analysis-phonetic
		/// A phonetic token filter that can be configured with different encoder types: 
		/// metaphone, doublemetaphone, soundex, refinedsoundex, caverphone1, caverphone2, cologne, nysiis, koelnerphonetik, haasephonetik, beidermorse
		///
		/// The replace parameter (defaults to true) controls if the token processed should be replaced with the encoded one (set it to true), or added (set it to false).
		/// </summary>
		/// <param name="name"></param>
		public PhoneticTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Phonetic;
		}


		/// <summary>
		/// A phonetic token filter that can be configured with different encoder types: 
		/// metaphone, doublemetaphone, soundex, refinedsoundex, caverphone1, caverphone2, cologne, nysiis, koelnerphonetik, haasephonetik, beidermorse
		/// </summary>
		public PhoneticEncoder Encoder
		{
			get { return _encoder; }
			set
			{
				_encoder = value;
				_encoderSet = true;
			}
		}

		/// <summary>
		/// The replace parameter (defaults to true) controls if the token processed should be replaced with the encoded one (set it to true), or added (set it to false).
		/// </summary>
		public bool Replace
		{
			get { return _replace; }
			set
			{
				_replace = value;
				_replaceSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("encoder", _encoder.ToString(), elasticsearchCrudJsonWriter, _encoderSet);
			JsonHelper.WriteValue("replace", _replace, elasticsearchCrudJsonWriter, _replaceSet);
		}
	}

	public enum PhoneticEncoder
	{
		metaphone, doublemetaphone, soundex, refinedsoundex, caverphone1, caverphone2, cologne, nysiis, koelnerphonetik, haasephonetik, beidermorse
	}
}