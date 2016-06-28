using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class LimitTokenFilter : AnalysisFilterBase
	{
		private bool _consumeAllTokens;
		private int _maxTokenCount;
		private bool _maxTokenCountSet;
		private bool _consumeAllTokensSet;

		/// <summary>
		/// Limits the number of tokens that are indexed per document and field.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public LimitTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Limit;
		}

		/// <summary>
		/// max_token_count
		/// The maximum number of tokens that should be indexed per document and field. The default is 1
		/// </summary>
		public int MaxTokenCount
		{
			get { return _maxTokenCount; }
			set
			{
				_maxTokenCount = value;
				_maxTokenCountSet = true;
			}
		}

		/// <summary>
		/// consume_all_tokens
		/// If set to true the filter exhaust the stream even if max_token_count tokens have been consumed already. The default is false.
		/// </summary>
		public bool ConsumeAllTokens
		{
			get { return _consumeAllTokens; }
			set
			{
				_consumeAllTokens = value;
				_consumeAllTokensSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max_token_count", _maxTokenCount, elasticsearchCrudJsonWriter, _maxTokenCountSet);
			JsonHelper.WriteValue("consume_all_tokens", _consumeAllTokens, elasticsearchCrudJsonWriter, _consumeAllTokensSet);
		}
	}
}