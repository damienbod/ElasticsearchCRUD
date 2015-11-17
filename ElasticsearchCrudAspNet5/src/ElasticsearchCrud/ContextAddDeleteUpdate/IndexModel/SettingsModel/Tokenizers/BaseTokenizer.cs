using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
	public abstract class BaseTokenizer : AnalysisTokenizerBase
	{
		private int _maxTokenLength;
		private bool _maxTokenLengthSet;
	
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
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max_token_length", _maxTokenLength, elasticsearchCrudJsonWriter, _maxTokenLengthSet);
		}
	}
}