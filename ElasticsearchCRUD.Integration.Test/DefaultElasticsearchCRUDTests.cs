using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
    public class DefaultElasticsearchCRUDTests
	{
		private List<SkillTestEntity> _entitiesForTests;
		private List<SkillTestEntityTwo> _entitiesForTestsTypeTwo;
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

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

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

				entityResult.Wait();
				var secondDelete = context.DeleteIndexAsync<SkillTestEntityTwo>();
				secondDelete.Wait();
			}
		}

		[Test]
		public void TestDefaultContextAdd100Entities()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateEntity(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		[ExpectedException(typeof(AggregateException))]
		public void TestDefaultContextAddEntitySaveChangesAsyncBadUrl()
		{
			using (var context = new ElasticSearchContext("http://locaghghghlhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(_entitiesForTests[1], 1);
				var ret = context.SaveChangesAsync();
				Console.WriteLine(ret.Result.Status);
			}
		}

		[Test]
		[ExpectedException(typeof(HttpRequestException))]
		public void TestDefaultContextAddEntitySaveChangesBadUrl()
		{
			using (var context = new ElasticSearchContext("http://locaghghghlhost:9200/", _elasticSearchMappingResolver))
			{

				context.AddUpdateEntity(_entitiesForTests[1], 1);
				context.SaveChanges();
			}
		}

		[Test]
		public void TestDefaultContextGetEntityAsync()
		{
			const int entityId = 34;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				var entityResult = context.GetEntityAsync<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
				Assert.IsNotNull(entityResult.Result.PayloadResult as SkillTestEntity);
			}
		}

		[Test]
		public void TestDefaultContextGetEntity()
		{
			const int entityId = 34;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				// Get the entity
				var entityResult = context.GetEntity<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException), ExpectedMessage = "HttpStatusCode.NotFound")]
		public void TestDefaultContextGetEntityNotFound()
		{
			const int entityId = 39994;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				// Get the entity
				var entityResult = context.GetEntity<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException), ExpectedMessage = "HttpStatusCode.NotFound")]
		public void TestDefaultContextGetEntityIndexNotFound()
		{
			const int entityId = 39994;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				// Get the entity
				var entityResult = context.GetEntity<SkillTestEntityNoIndex>(entityId);
				Assert.AreEqual(entityResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(HttpRequestException))]
		public void TestDefaultContextGetEntityBadUrl()
		{
			const int entityId = 34;
			using (var context = new ElasticSearchContext("http://localghghghhost:9200/", _elasticSearchMappingResolver))
			{
				var entityResult = context.GetEntity<SkillTestEntity>(entityId);;
			}
		}

		[Test]
		public void TestDefaultContextUpdateEntity()
		{
			const int entityId = 34;
			SkillTestEntity resultfromGet;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				// add a new entity
				context.AddUpdateEntity(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				resultfromGet = context.GetEntityAsync<SkillTestEntity>(entityId).Result.PayloadResult;

			}

			resultfromGet.Name = "updated";
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				// update entity
				context.AddUpdateEntity(resultfromGet, entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				var readEntity = context.GetEntityAsync<SkillTestEntity>(entityId).Result.PayloadResult;
				Assert.AreEqual(readEntity.Name, resultfromGet.Name);
				Assert.AreNotEqual(readEntity.Name, _entitiesForTests[entityId]);
			}
		}

		[Test]
		public void TestDefaultContextDeleteEntity()
		{
			const int entityId = 35;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(HttpStatusCode.OK, ret.Result.Status);

				// Get the entity
				var entityResult = context.GetEntityAsync<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
				Assert.IsNotNull(entityResult.Result.PayloadResult as SkillTestEntity);

				// Delete the entity
				context.DeleteEntity<SkillTestEntity>(entityId);
				var deleteResult = context.SaveChangesAsync();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException))]
		public void TestDefaultContextDeleteEntityWhichDoesNotExist()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{

				// Delete the entity
				context.DeleteEntity<SkillTestEntity>(6433);
				context.DeleteEntity<SkillTestEntity>(22222);

				var task = Task.Run(() => context.SaveChangesAsync());

			try
			{
				task.Wait();
				}
				catch (AggregateException ae)
				{

					ae.Handle((x) =>
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
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var entityResult = context.GetEntityAsync<SkillTestEntity>(entityId);
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.NotFound);
			}
		}

		[Test]
		public void TestDefaultContextSaveWithNoChanges()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var entityResult = context.SaveChangesAsync();
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		public void TestDefaultContextDeleteIndex()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateEntity(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SkillTestEntityNoIndex>();
				entityResult.Wait();
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.NotFound);
			}
		}

		[Test]
		public void TestDefaultContextDeleteIndexNotFound()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
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
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

				try
				{
					entityResult.Wait();
				}
				catch (AggregateException ae)
				{

					ae.Handle((x) =>
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
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateEntity(_entitiesForTests[i], i);
					context.AddUpdateEntity(_entitiesForTestsTypeTwo[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}
		}

	}
}
