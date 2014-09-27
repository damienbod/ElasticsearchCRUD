using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
    public class DefaultElasticsearchCRUDTests
	{
		private List<SkillTestEntity> _entitiesForTests;
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();
		[SetUp]
		public void SetUp()
		{
			_entitiesForTests = new List<SkillTestEntity>();
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
			}
		}

		[TearDown]
		public void TearDown()
		{
			_entitiesForTests = null;
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
		public void TestDefaultContextGetEntity()
		{
			const int entityId = 34;
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(_entitiesForTests[entityId], entityId);

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				var entityResult = context.GetEntity<SkillTestEntity>(entityId);
				Assert.AreEqual(entityResult.Result.Status, HttpStatusCode.OK);
				Assert.AreEqual(entityResult.Result.PayloadResult.Id, entityId);
				Assert.IsNotNull(entityResult.Result.PayloadResult as SkillTestEntity);
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
				resultfromGet = context.GetEntity<SkillTestEntity>(entityId).Result.PayloadResult;

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
				var readEntity = context.GetEntity<SkillTestEntity>(entityId).Result.PayloadResult;
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
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);

				// Get the entity
				var entityResult = context.GetEntity<SkillTestEntity>(entityId);
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

				var task1 = context.SaveChangesAsync();

				try
				{
					task1.Wait();
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
				var entityResult = context.GetEntity<SkillTestEntity>(entityId);
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
	}

	public class SkillTestEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}
}
