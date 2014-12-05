
using System;
using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
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
			analysis.AnalysisAnalyzer.SetAnalyzer("blocks_analyzer", DefaultAnalyzers.Custom);
			analysis.AnalysisAnalyzer.Tokenizer = "whitespace";
			analysis.AnalysisAnalyzer.Filter = new List<string> { "lowercase", "blocks_filter", "shingle" };

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
			analysis.AnalysisAnalyzer.SetAnalyzer("default", DefaultAnalyzers.Standard);
			analysis.AnalysisAnalyzer.Tokenizer = "standard";
			analysis.AnalysisAnalyzer.Filter = new List<string> { "standard", "my_ascii_folding" };
			analysis.AnalysisFilter.CustomFilters = new List<AnalysisFilterBase> { new AsciifoldingFilter("my_ascii_folding") { PreserveOriginal = true } };

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson, result);

		}
	}
}
