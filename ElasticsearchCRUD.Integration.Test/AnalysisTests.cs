
using System;
using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
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
			const string targetJson = "\"analyzer\":{\"blocks_analyzer\":{\"type\":\"custom\",\"tokenizer\":\"whitespace\",\"filter\":[\"lowercase\",\"blocks_filter\",\"shingle\"]}}";
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
	}
}
