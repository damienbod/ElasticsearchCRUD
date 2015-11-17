using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	//	"filter" : {
	//		"blocks_filter" : {
	//			"type" : "word_delimiter",
	//			"preserve_original": "true"
	//		},
	//	   "shingle":{
	//		   "type":"shingle",
	//		   "max_shingle_size":5,
	//		   "min_shingle_size":2,
	//		   "output_unigrams":"true"
	//		},
	//		"filter_stop":{
	//		   "type":"stop",
	//		   "enable_position_increments":"false"
	//		}
	//	},
	public class AnalysisFilter
	{
		private List<AnalysisFilterBase> _customFilters;
		private bool _customFiltersSet;

		public List<AnalysisFilterBase> CustomFilters
		{
			get { return _customFilters; }
			set
			{
				_customFilters = value;
				_customFiltersSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_customFiltersSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				foreach (var item in _customFilters)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}