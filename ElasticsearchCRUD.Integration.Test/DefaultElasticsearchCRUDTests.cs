using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
    public class DefaultElasticsearchCrudTests
	{
		private List<SkillTestEntity> _entitiesForTests;
		private List<SkillTestEntityTwo> _entitiesForTestsTypeTwo;
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
			_entitiesForTests = new List<SkillTestEntity>();
			_entitiesForTestsTypeTwo = new List<SkillTestEntityTwo>();
			// Create a 100 entities
			for (int i = 0; i < 100; i++)
			{
				var entity = new SkillTestEntity
				{
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					Description = "A test entity description",
					Id = i,
					Name = "cool"
				};

				_entitiesForTests.Add(entity);

				var entityTwo = new SkillTestEntityTwo
				{
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					Description = "A test entity description",
					Id = i,
					Name = "cool"
				};

				_entitiesForTestsTypeTwo.Add(entityTwo);
			}
		}

		[TearDown]
		public void TearDown()
		{
			_entitiesForTests = null;

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

				entityResult.Wait();
				var secondDelete = context.DeleteIndexAsync<SkillTestEntityTwo>();
				secondDelete.Wait();

				var thirdDelete = context.DeleteIndexAsync<TestJsonIgnore>();
				thirdDelete.Wait();
			}
		}

		[Test]
		public void TestDefaultContextAdd100Entities()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		public void TestDefaultContextCount()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 0; i < 7; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
				long found = 0;
				Task.Run(() =>
				{
					while (true)
					{
						Thread.Sleep(300);
						found = context.Count<SkillTestEntity>();
						if (found == 7)
						{
							_resetEvent.Set();
						}	
					}					
				});

				// allow elasticsearch time to update...
				WaitForDataOrFail();

				Assert.AreEqual(7, found);
			}
		}
		
		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextCount: Index not found")]
		public void TestDefaultContextCountWithNoIndex()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				 context.Count<SkillTestEntityNoIndex>();
			}
		}

		[Test]
		[ExpectedException(typeof(AggregateException))]
		public void TestDefaultContextAddEntitySaveChangesAsyncBadUrl()
		{
			using (var context = new ElasticsearchContext("http://locaghghghlhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[1], 1);
				var ret = context.SaveChangesAsync();
				Console.WriteLine(ret.Result.Status);
			}
		}

		[Test]
		[ExpectedException(typeof(HttpRequestException))]
		public void TestDefaultContextAddEntitySaveChangesBadUrl()
		{
			using (var context = new ElasticsearchContext("http://locaghghghlhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[1], 1);
				context.SaveChanges();
			}
		}

		[Test]
		public void TestDefaultContextGetEntityAsync()
		{
			const int entityId = 34;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				var entityResult = context.GetDocumentAsync<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
				Assert.IsNotNull(entityResult.Result.PayloadResult);
			}
		}

		[Test]
		public void TestDefaultContextGetEntity()
		{
			const int entityId = 34;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				// Get the entity
				var entityResult = context.GetDocument<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Id, entityId);
			}
		}

		[Test]
		public void TestDefaultContextSearchExists()
		{
			const int documentId = 34;
			string searchJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[documentId], documentId);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
	
			}

			Task.Run(() =>
			{
				using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
				{
					while (true)
					{
						Thread.Sleep(300);
						var exists = context.SearchExists<SkillTestEntity>(searchJson);
						if (exists)
						{
							_resetEvent.Set();
						}
					}
				}
			});

			// allow elasticsearch time to update...
			WaitForDataOrFail();
		}

		[Test]
		public void TestDefaultContextSearchNotExists()
		{
			const int documentId = 3574474;
			string searchJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				// Get the entity
				var exists = context.SearchExists<SkillTestEntity>(searchJson);
				Assert.IsFalse(exists);
			}
		}

		[Test]
		public void TestDefaultContextSearchMatchAll()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[34], 34);
				context.AddUpdateDocument(_entitiesForTests[35], 35);
				context.SaveChanges();
			}

			Task.Run(() =>
			{
				using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
				{
					while (true)
					{
						Thread.Sleep(300);
						var exists = context.SearchExists<SkillTestEntity>(BuildSearchMatchAll());
						if (exists)
						{
							_resetEvent.Set();
						}
					}
				}
			});

			WaitForDataOrFail();
		}

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

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticSearchContextGet: HttpStatusCode.NotFound")]
		public void TestDefaultContextGetEntityNotFound()
		{
			const int entityId = 39994;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				// Get the entity
				var entityResult = context.GetDocument<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticSearchContextGet: HttpStatusCode.NotFound")]
		public void TestDefaultContextGetEntityIndexNotFound()
		{
			const int entityId = 39994;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				// Get the entity
				var entityResult = context.GetDocument<SkillTestEntityNoIndex>(entityId);
				Assert.AreEqual(entityResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(HttpRequestException))]
		public void TestDefaultContextGetEntityBadUrl()
		{
			const int entityId = 34;
			using (var context = new ElasticsearchContext("http://localghghghhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.GetDocument<SkillTestEntity>(entityId);
			}
		}

		[Test]
		public void TestDefaultContextUpdateEntity()
		{
			const int entityId = 34;
			SkillTestEntity resultfromGet;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				// add a new entity
				context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				resultfromGet = context.GetDocumentAsync<SkillTestEntity>(entityId).Result.PayloadResult;

			}

			resultfromGet.Name = "updated";
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				// update entity
				context.AddUpdateDocument(resultfromGet, entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				var readEntity = context.GetDocumentAsync<SkillTestEntity>(entityId).Result.PayloadResult;
				Assert.AreEqual(readEntity.Name, resultfromGet.Name);
				Assert.AreNotEqual(readEntity.Name, _entitiesForTests[entityId]);
			}
		}

		[Test]
		public void TestDefaultContextDeleteEntity()
		{
			const int entityId = 35;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(HttpStatusCode.OK, ret.Result.Status);

				// Get the entity
				var entityResult = context.GetDocumentAsync<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
				Assert.IsNotNull(entityResult.Result.PayloadResult);

				// Delete the entity
				context.DeleteDocument<SkillTestEntity>(entityId);
				context.SaveChanges();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException))]
		public void TestDefaultContextDeleteEntityWhichDoesNotExist()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				// Delete the entity
				context.DeleteDocument<SkillTestEntity>(6433);
				context.DeleteDocument<SkillTestEntity>(22222);

				var task = Task.Run(() => context.SaveChangesAsync());

			try
			{
				task.Wait();
				}
				catch (AggregateException ae)
				{

					ae.Handle(x =>
					{
						if (x is ElasticsearchCrudException) // This is what we expect.
						{
							throw x;
						}
						return false; // stop.
					});
				}
			}
		}

		[Test]
		public void TestDefaultContextGetEntityWhichDoesNotExist()
		{
			const int entityId = 3004;
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var entityResult = context.GetDocumentAsync<SkillTestEntity>(entityId);
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.NotFound);
			}
		}

		[Test]
		public void TestDefaultContextSaveWithNoChanges()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var entityResult = context.SaveChangesAsync();
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		public void TestDefaultContextDeleteIndex()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SkillTestEntityNoIndex>();
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.NotFound);
			}
		}

		[Test]
		public void TestDefaultContextDeleteIndexNotFound()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SkillTestEntityNoIndex>();
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.NotFound);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException))]
		public void TestDefaultContextDeleteIndexNotActivated()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

				try
				{
					entityResult.Wait();
				}
				catch (AggregateException ae)
				{

					ae.Handle(x =>
					{
						if (x is ElasticsearchCrudException) // This is what we expect.
						{
							throw x;
						}
						return false; // stop.
					});
				}

				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		public void TestDefaultContextAdd100EntitiesForTwoTypes()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
					context.AddUpdateDocument(_entitiesForTestsTypeTwo[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticSearchContextGet: HttpStatusCode.NotFound")]
		public void TestDefaultContextDeleteByQuerySingleDocumentWithId()
		{
			const int documentId = 153;
			string deleteJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 150; i < 160; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i-150], i);
				}
				// Save to Elasticsearch
				var ret = context.SaveChanges();

				// Wait for Elasticsearch to update
				long foundBefore = 0;

				Task.Run(() =>
				{
					while (true)
					{
						Thread.Sleep(300);
						foundBefore = context.Count<SkillTestEntity>();
						if (foundBefore > 9)
						{
							_resetEvent.Set();
						}
					}
				});

				// allow elasticsearch time to update...
				WaitForDataOrFail();
							
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
				context.DeleteByQuery<SkillTestEntity>(deleteJson);

				// Clear thecache so count or get returns the latest value
				context.ClearCacheForIndex<SkillTestEntity>();
				long foundAfter = context.Count<SkillTestEntity>();

				Console.WriteLine("found before {0}, after {1}", foundBefore, foundAfter);
				Assert.Greater(foundBefore, foundAfter);
				context.GetDocument<SkillTestEntity>(documentId);
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticSearchContextGet: HttpStatusCode.NotFound")]
		public void TestDefaultContextDeleteByQueryForTwoDocumentsWithIdQuery()
		{
			const int documentId153 = 153;
			const int documentId155 = 155;

			string deleteJson = "{\"query\": { \"ids\": { \"values\":  [\"" + documentId153 + "\", \"" + documentId155 + "\"]  }}}";
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 150; i < 160; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i - 150], i);
				}
				// Save to Elasticsearch
				var ret = context.SaveChanges();

				// Wait for Elasticsearch to update
				long foundBefore = 0;

				Task.Run(() =>
				{
					while (true)
					{
						Thread.Sleep(300);
						foundBefore = context.Count<SkillTestEntity>();
						if (foundBefore > 9)
						{
							_resetEvent.Set();
						}
					}
				});

				// allow elasticsearch time to update...
				WaitForDataOrFail();

				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
				context.DeleteByQuery<SkillTestEntity>(deleteJson);

				// Clear thecache so count or get returns the latest value
				context.ClearCacheForIndex<SkillTestEntity>();
				var foundAfter = context.Count<SkillTestEntity>();

				Console.WriteLine("found before {0}, after {1}", foundBefore, foundAfter);
				Assert.Greater(foundBefore -1, foundAfter);
				context.GetDocument<SkillTestEntity>(documentId153);
			}
		}
		[Test]
		public void TestDefaultContextClearCache()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				for (int i = 150; i < 160; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i - 150], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				Assert.IsTrue(context.ClearCacheForIndex<SkillTestEntity>());
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextClearCache: Could nor clear cache for index skilltestentitynoindexs")]
		public void TestDefaultContextClearCacheForNoIndex()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.ClearCacheForIndex<SkillTestEntityNoIndex>());
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticSearchContextGet: HttpStatusCode.NotFound")]
		public void TestDefaultContextDeleteByQuerySingleDocumentWithNonExistingId()
		{
			const int documentId = 965428;
			string deleteJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				for (int i = 150; i < 160; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i - 150], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
				context.DeleteByQuery<SkillTestEntity>(deleteJson);
				context.GetDocument<SkillTestEntity>(documentId);
			}
		}

		[Test]
		public void TestDefaultContextTestJsonIgnore()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var testJsonIgnore = new TestJsonIgnore
				{
					MyStringArray = new List<string> {"ff", "dd"},
					BlahBlah = "sss",
					Id = 3,
					MyLongArray = new List<long> {23, 4323456, 333332},
					SkillSingleChildElement = new SkillSingleChildElement {Details = "ss", Id = 3},
					SkillSingleChildElementList =
						new List<SkillSingleChildElement> {new SkillSingleChildElement {Details = "ww", Id = 2}}
				};

				context.AddUpdateDocument(testJsonIgnore, testJsonIgnore.Id);

				// Save to Elasticsearch
				context.SaveChanges();

				var ret = context.GetDocument<TestJsonIgnore>(3);
				Assert.AreEqual(ret.MyLongArray, null);
				Assert.AreEqual(ret.SkillSingleChildElement, null);
				Assert.AreEqual(ret.SkillSingleChildElementList, null);
				Assert.AreEqual(ret.BlahBlahNull, null);
				Assert.AreEqual(ret.BlahBlah, "sss");
				Assert.AreEqual(ret.Id, 3);
			}
		}	
	}
}
