using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public class StandardAnaylzer : BaseStopAnalyzer
	{
		private int _maxTokenLength;
		private bool _maxTokenLengthSet;

		public StandardAnaylzer(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultAnalyzers.Standard;
		}

		/// <summary>
		/// max_token_length
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
			WriteCommonValues(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max_token_length", _maxTokenLength, elasticsearchCrudJsonWriter, _maxTokenLengthSet);

		}
	}
}
