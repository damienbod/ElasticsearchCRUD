using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class KeywordTokenizer : AnalysisTokenizerBase
	{
		// buffer_size
		private int _bufferSize;
		private bool _bufferSizeSet;

		/// <summary>
		/// A tokenizer of type keyword that emits the entire input as a single output.
		/// </summary>
		/// <param name="name">Name for the custom tokenizer</param>
		public KeywordTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.Keyword;
		}
		/// <summary>
		/// The maximum token length. If a token is seen that exceeds this length then it is discarded. Defaults to 255.
		/// </summary>
		public int BufferSize
		{
			get { return _bufferSize; }
			set
			{
				_bufferSize = value;
				_bufferSizeSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("buffer_size", _bufferSize, elasticsearchCrudJsonWriter, _bufferSizeSet);
		}
	}
}
