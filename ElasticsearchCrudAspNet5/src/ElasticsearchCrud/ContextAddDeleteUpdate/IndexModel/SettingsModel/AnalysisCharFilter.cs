using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.CharFilters;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	//"char_filter" : {
	//	   "my_mapping" : {
	//		   "type" : "mapping",
	//		   "mappings" : ["ph=>f", "qu=>k"]
	//	   }
	//},
	public class AnalysisCharFilter
	{
		private List<AnalysisCharFilterBase> _customCharFilters;
		private bool _customCharFiltersSet;

		public List<AnalysisCharFilterBase> CustomFilters
		{
			get { return _customCharFilters; }
			set
			{
				_customCharFilters = value;
				_customCharFiltersSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_customCharFiltersSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("char_filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				foreach (var item in _customCharFilters)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}