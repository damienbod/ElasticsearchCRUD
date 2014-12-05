using System;
using System.Collections.Generic;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers;
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
			var analysis = new Analysis
			{
				Analyzer =
				{
					Analyzers = new List<AnalyzerBase>
					{
						new CustomAnalyzer("blocks_analyzer")
						{
							Tokenizer = DefaultTokenizers.Whitespace,
							Filter = new List<string> {"lowercase", "blocks_filter", "shingle"}
						},

					}
				}
			};

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
				"\"analysis\":{\"filter\":{\"my_ascii_folding\":{\"type\":\"asciifolding\",\"preserve_original\":true}},\"analyzer\":{\"default\":{\"type\":\"custom\",\"tokenizer\":\"standard\",\"filter\":[\"standard\",\"my_ascii_folding\"]}}}";
		
			var analysis = new Analysis
			{
				Analyzer =
				{
					Analyzers = new List<AnalyzerBase>
					{
						new CustomAnalyzer("default")
						{
							Tokenizer = DefaultTokenizers.Standard,
							Filter = new List<string> {"standard", "my_ascii_folding"}
						}
					}
				},
				Filters =
				{
					CustomFilters = new List<AnalysisFilterBase> {new AsciifoldingFilter("my_ascii_folding") {PreserveOriginal = true}}
				}
			};

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
				"\"analysis\":{\"tokenizer\":{\"bigger_limit_token_count\":{\"type\":\"standard\",\"max_token_length\":360}},\"analyzer\":{\"default\":{\"type\":\"custom\",\"tokenizer\":\"bigger_limit_token_count\"}}}";

			var analysis = new Analysis
			{
				Analyzer =
				{
					Analyzers = new List<AnalyzerBase>
					{
						new CustomAnalyzer("default")
						{
							Tokenizer = "bigger_limit_token_count"
						}
					}
				},
				Tokenizers =
				{
					CustomTokenizers = new List<AnalysisTokenizerBase>
					{
						new StandardTokenizer("bigger_limit_token_count") {MaxTokenLength = 360}
					}
				}
			};

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson, result);
		}

		[Test]
		public void SerializeAnalyzerWithEdgeNGramTokenizer()
		{
			const string targetJson =
				"\"analysis\":{\"tokenizer\":{\"my_edgengram_tokenizer\":{\"type\":\"edgeNGram\",\"min_gram\":2,\"max_gram\":4,\"token_chars\":[\"digit\",\"letter\"]}},\"analyzer\":{\"default\":{\"type\":\"custom\",\"tokenizer\":\"my_edgengram_tokenizer\"}}}";

			var analysis = new Analysis
			{
				Analyzer =
				{
					Analyzers = new List<AnalyzerBase>
					{
						new CustomAnalyzer("default")
						{
							Tokenizer = "my_edgengram_tokenizer"
						}
					}
				},
				Tokenizers =
				{
					CustomTokenizers = new List<AnalysisTokenizerBase>
					{
						new EdgeNGramTokenizer("my_edgengram_tokenizer")
						{
							MaxGram = 4,
							MinGram = 2,
							TokenChars = new List<TokenChar> {TokenChar.digit, TokenChar.letter}
						}
					}
				}
			};

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson, result);
		}

		[Test]
		public void SerializeAnalyzerWithNGramTokenizer()
		{
			const string targetJson =
				"\"analysis\":{\"tokenizer\":{\"my_ngram_tokenizer\":{\"type\":\"nGram\",\"min_gram\":1,\"max_gram\":5,\"token_chars\":[\"digit\",\"letter\"]}},\"analyzer\":{\"default\":{\"type\":\"custom\",\"tokenizer\":\"my_ngram_tokenizer\"}}}";

			var analysis = new Analysis
			{
				Analyzer =
				{
					Analyzers = new List<AnalyzerBase>
					{
						new CustomAnalyzer("default")
						{
							Tokenizer = "my_ngram_tokenizer"
						}
					}
				},
				Tokenizers =
				{
					CustomTokenizers = new List<AnalysisTokenizerBase>
					{
						new NGramTokenizer("my_ngram_tokenizer")
						{
							MaxGram = 5,
							MinGram = 1,
							TokenChars = new List<TokenChar> {TokenChar.digit, TokenChar.letter}
						}
					}
				}
			};

			var jsonWriter = new ElasticsearchCrudJsonWriter();
			analysis.WriteJson(jsonWriter);
			var result = jsonWriter.GetJsonString();
			Console.WriteLine(result);
			Assert.AreEqual(targetJson, result);
		}
	}
}
