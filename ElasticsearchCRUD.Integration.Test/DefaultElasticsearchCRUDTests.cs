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
		private List<SkillTestEntity> _entitiesForTests = new List<SkillTestEntity>();
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();
		[SetUp]
		public void SetUp()
		{
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
