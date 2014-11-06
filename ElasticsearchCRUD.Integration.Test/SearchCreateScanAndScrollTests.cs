using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	public class SearchCreateScanAndScrollTests
	{
		private List<ScanScrollTypeV1> _entitiesForTests;
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);

		private void WaitForDataOrFail()
		{
			if (!_resetEvent.WaitOne(5000))
			{
				Assert.Fail("No data received within specified time");
			}
		}
		[SetUp]
		public void SetUp()
		{
			_entitiesForTests = new List<ScanScrollTypeV1>();
			// Create a 100 entities
			for (int i = 0; i < 10000; i++)
			{
				var entity = new ScanScrollTypeV1
				{
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					Description = "A test entity description",
					Id = i,
					Name = "cool"
				};

				_entitiesForTests.Add(entity);
			}
		}

		[TearDown]
		public void TearDown()
		{
			_entitiesForTests = null;

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<ScanScrollTypeV1>();
				entityResult.Wait();

				var entityResultV2 = context.DeleteIndexAsync<ScanScrollTypeV2>();
				entityResultV2.Wait();
			}
		}

		[Test]
		public void TestScanAndScollReindexFor1000Entities()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 0; i < 10000; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				// lets elasticsearch have time to update
				Thread.Sleep(2000);
				context.ClearCacheForIndex<ScanScrollTypeV1>();
				var scanScrollConfig = new ScanAndScrollConfiguration(1, TimeUnits.Second, 300);
				var result = context.SearchCreateScanAndScroll<ScanScrollTypeV1>(BuildSearchMatchAll(), scanScrollConfig);

				var scrollId = result.ScrollId;

				int processedResults = 0;
				while (result.TotalHits > processedResults)
				{
					var resultCollection = context.Search<ScanScrollTypeV1>("", scrollId, scanScrollConfig);
					scrollId = resultCollection.ScrollId;

					foreach (var item in resultCollection.PayloadResult)
					{
						processedResults++;
						context.AddUpdateDocument(ConvertScanScrollTypeV1(item), item.Id);
					}
					context.SaveChanges();
					
				}

				// lets elasticsearch have time to update
				Thread.Sleep(2000);
				context.ClearCacheForIndex<ScanScrollTypeV2>();
				Assert.AreEqual(10000, context.Count<ScanScrollTypeV2>());
			}
		}

		private ScanScrollTypeV2 ConvertScanScrollTypeV1(ScanScrollTypeV1 item)
		{
			return new ScanScrollTypeV2
			{
				Created = item.Created,
				Updated = item.Updated,
				Description = item.Description,
				Id = item.Id,
				Name = item.Name,
				Vorname = "Yes we are reindexing"
			};
		}

		//{
		//	"query" : {
		//		"match_all" : {}
		//	}
		//}
		private string BuildSearchMatchAll()
		{
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"match_all\" : {}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}
	}

	public class ScanScrollTypeV1
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}

	public class ScanScrollTypeV2
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Vorname { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}
}
