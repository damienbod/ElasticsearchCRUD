using ElasticsearchCRUD.Model;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class StandardTokenizer : BaseTokenizer
	{
		/// <summary>
		/// The maximum token length. If a token is seen that exceeds this length then it is discarded. Defaults to 255.
		/// </summary>
		/// <param name="name">name of the custom tokenizer ToLower()</param>
		public StandardTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.Standard;
		}
	}
}
