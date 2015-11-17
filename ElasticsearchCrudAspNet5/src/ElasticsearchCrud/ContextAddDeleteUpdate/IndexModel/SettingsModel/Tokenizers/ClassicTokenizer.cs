using ElasticsearchCRUD.Model;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class ClassicTokenizer : BaseTokenizer
	{
		/// <summary>
		/// A tokenizer of type classic providing grammar based tokenizer that is a good tokenizer for English language documents. 
		/// This tokenizer has heuristics for special treatment of acronyms, company names, email addresses, and internet host names. 
		/// However, these rules don’t always work, and the tokenizer doesn’t work well for most languages other than English.
		/// </summary>
		/// <param name="name">name of the custom tokenizer ToLower()</param>
		public ClassicTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.Classic;
		}
	}
}