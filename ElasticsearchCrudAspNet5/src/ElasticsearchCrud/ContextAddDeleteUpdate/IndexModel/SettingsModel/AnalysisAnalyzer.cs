using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class AnalysisAnalyzer
	{
		private List<AnalyzerBase> _analyzers;
		private bool _analyzersSet;

		public List<AnalyzerBase> Analyzers
		{
			get { return _analyzers; }
			set
			{
				_analyzers = value;
				_analyzersSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_analyzersSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("analyzer");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				foreach (var item in _analyzers)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}
