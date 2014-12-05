using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public class StandardTokenizer : AnalysisTokenizerBase
	{
		private int _maxTokenLength;
		private bool _maxTokenLengthSet;

		public StandardTokenizer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenizers.Standard;
		}
		/// <summary>
		/// The maximum token length. If a token is seen that exceeds this length then it is discarded. Defaults to 255.
		/// </summary>
		public int MaxTokenLength
		{
			get { return _maxTokenLength; }
			set
			{
				_maxTokenLength = value;
				_maxTokenLengthSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			//"preserve_original" : true
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max_token_length", _maxTokenLength, elasticsearchCrudJsonWriter, _maxTokenLengthSet);
		}
	}
}
