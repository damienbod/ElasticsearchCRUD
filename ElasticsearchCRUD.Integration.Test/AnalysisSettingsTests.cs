using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class AnalysisSettingsTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				if (context.IndexExists<TestSettingsIndex>())
				{
					context.AllowDeleteForIndex = true;
					context.DeleteIndex<TestSettingsIndex>();
				}
			}
		}
		
		//"settings": {
		//	"analysis": {
		//		"analyzer": {
		//			"alfeanalyzer": {
		//				"type": "pattern",
		//				"pattern": "\\s+"
		//			}
		//		}
		//	}
		//},
		[Test]
		public void CreateNewIndexDefinitionForPatternAnalyzer()
		{
			var testSettingsIndexDefinition = new IndexDefinition
			{
				IndexSettings =
				{
					Analysis = new Analysis
					{
						Analyzer =
						{
							Analyzers = new List<AnalyzerBase>
							{
								new PatternAnalyzer("alfeanalyzer")
								{
									Pattern = "\\s+"
								}
							}
						}
					},
					NumberOfShards = 3,
					NumberOfReplicas = 1
				},
			};

			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var result = context.IndexCreate<TestSettingsIndex>(testSettingsIndexDefinition);

				Thread.Sleep(1500);
			
				Assert.IsTrue(context.IndexExists<TestSettingsIndex>());

				//Assert.GreaterOrEqual(result.PayloadResult.Shards.Successful, 1);
			}
		}

	}

	public class TestSettingsIndex
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
