
using System;
using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers;
using ElasticsearchCRUD.Model;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class AnalysisTests
	{
		/// "analyzer" : {
		///		"blocks_analyzer" : {
		///			"type" : "custom",
		///			"tokenizer" : "whitespace",
		///			"filter" : ["lowercase", "blocks_filter", "shingle"]
		///		}
		///	}
		[Test]
		public void SerializeBlocksAnalyzer()
		{
			const string targetJson = "\"analysis\":{\"analyzer\":{\"blocks_analyzer\":{\"type\":\"custom\",\"tokenizer\":\"whitespace\",\"filter\":[\"lowercase\",\"blocks_filter\",\"shingle\"]}}}";
			var analysis = new Analysis();
			analysis.Analyzer.SetAnalyzer("blocks_analyzer", DefaultAnalyzers.Custom);
			analysis.Analyzer.Tokenizer = "whitespace";
			analysis.Analyzer.Filter = new List<string> { "lowercase", "blocks_filter", "shingle" };

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson,result);

		}

		//	"analysis" : {
		//	"analyzer" : {
		//		"default" : {
		//			"tokenizer" : "standard",
		//			"filter" : ["standard", "my_ascii_folding"]
		//		}
		//	},
		//	"filter" : {
		//		"my_ascii_folding" : {
		//			"type" : "asciifolding",
		//			"preserve_original" : true
		//		}
		//	}
		//}
		[Test]
		public void SerializeAnalyzerWithAsciifoldingFilter()
		{
			const string targetJson =
				"\"analysis\":{\"filter\":{\"my_ascii_folding\":{\"type\":\"asciifolding\",\"preserve_original\":true}},\"analyzer\":{\"default\":{\"type\":\"standard\",\"tokenizer\":\"standard\",\"filter\":[\"standard\",\"my_ascii_folding\"]}}}";
		
			var analysis = new Analysis();
			analysis.Analyzer.SetAnalyzer("default", DefaultAnalyzers.Standard);
			analysis.Analyzer.Tokenizer = "standard";
			analysis.Analyzer.Filter = new List<string> { "standard", "my_ascii_folding" };
			analysis.Filters.CustomFilters = new List<AnalysisFilterBase> { new AsciifoldingFilter("my_ascii_folding") { PreserveOriginal = true } };

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson, result);

		}

		[Test]
		public void SerializeAnalyzerWithStandardTokenizer()
		{
			const string targetJson =
				"\"analysis\":{\"tokenizer\":{\"bigger_limit_token_count\":{\"type\":\"standard\",\"max_token_length\":360}},\"analyzer\":{\"default\":{\"type\":\"standard\",\"tokenizer\":\"bigger_limit_token_count\"}}}";
		
			var analysis = new Analysis();
			analysis.Analyzer.SetAnalyzer("default", DefaultAnalyzers.Standard);
			analysis.Analyzer.Tokenizer = "bigger_limit_token_count";
			analysis.Tokenizers.CustomTokenizers = new List<AnalysisTokenizerBase>
			{
				new StandardTokenizer("bigger_limit_token_count") {MaxTokenLength = 360}
			};

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson, result);
		}

	}
}
