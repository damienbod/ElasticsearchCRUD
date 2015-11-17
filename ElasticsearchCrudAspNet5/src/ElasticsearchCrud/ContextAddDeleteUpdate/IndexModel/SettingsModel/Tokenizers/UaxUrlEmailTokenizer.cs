using ElasticsearchCRUD.Model;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class UaxUrlEmailTokenizer : BaseTokenizer
	{
		/// <summary>
		/// A tokenizer of type uax_url_email which works exactly like the standard tokenizer, but tokenizes emails and urls as single tokens.
		/// </summary>
		/// <param name="name">name of the custom tokenizer (ToLower()</param>
		public UaxUrlEmailTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.UaxUrlEmail;
		}
	}
}