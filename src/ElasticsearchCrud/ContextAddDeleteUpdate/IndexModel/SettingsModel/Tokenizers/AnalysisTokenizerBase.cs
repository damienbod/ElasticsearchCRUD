using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
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
	public abstract class AnalysisTokenizerBase
	{
		protected bool AnalyzerSet;
		protected string Name;
		protected string Type;

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		protected virtual void WriteJsonBase(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, Action<ElasticsearchCrudJsonWriter> writeFilterSpecific)
		{
			if (AnalyzerSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(Name);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				writeFilterSpecific.Invoke(elasticsearchCrudJsonWriter);

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}
