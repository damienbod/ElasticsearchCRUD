using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class AnalysisTokenizer
	{
		private List<AnalysisTokenizerBase> _customTokenizers;
		private bool _customFiltersSet;

		public List<AnalysisTokenizerBase> CustomTokenizers
		{
			get { return _customTokenizers; }
			set
			{
				_customTokenizers = value;
				_customFiltersSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_customFiltersSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("tokenizer");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				foreach (var item in _customTokenizers)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}