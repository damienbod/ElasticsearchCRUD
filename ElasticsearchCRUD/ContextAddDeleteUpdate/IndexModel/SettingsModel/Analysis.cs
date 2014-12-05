namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	// TODO "tokenizer": 
   // "analysis" : {
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
   //	"analyzer" : {
   //		"blocks_analyzer" : {
   //			"type" : "custom",
   //			"tokenizer" : "whitespace",
   //			"filter" : ["lowercase", "blocks_filter", "shingle"]
   //		}
   //	}
   //}
	public class Analysis
	{
		public Analysis()
		{
			AnalysisFilter = new AnalysisFilter();
			AnalysisAnalyzer = new AnalysisAnalyzer();
			AnalysisTokenizer = new AnalysisTokenizer();
		}

		public AnalysisTokenizer AnalysisTokenizer { get; set; }
		public AnalysisFilter AnalysisFilter { get; set; }
		public AnalysisAnalyzer AnalysisAnalyzer { get; set; }

		public virtual void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			//WriteJson("number_of_replicas", _numberOfReplicas, elasticsearchCrudJsonWriter, _numberOfReplicasSet);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("analysis");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			AnalysisTokenizer.WriteJson(elasticsearchCrudJsonWriter);
			AnalysisFilter.WriteJson(elasticsearchCrudJsonWriter);
			AnalysisAnalyzer.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}