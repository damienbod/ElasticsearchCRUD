using ElasticsearchCRUD.Model;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class EdgeNGramTokenizer : BaseNGramTokenizer
	{
		/// <summary>
		/// A tokenizer of type edgeNGram.
		/// This tokenizer is very similar to nGram but only keeps n-grams which start at the beginning of a token.
		/// </summary>
		/// <param name="name">name of the custom tokenizer (ToLower()</param>
		public EdgeNGramTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.EdgeNGram;
		}
	}

	public enum TokenChar
	{
		letter,
		digit,
		whitespace,
		punctuation,
		symbol 
	}
}
