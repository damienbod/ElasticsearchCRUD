using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class SpecialMappingElasticsearchCRUDTests
	{
		private List<SkillTestEntity> _entitiesForTests;
		private List<SkillTestEntityTwo> _entitiesForTestsTypeTwo;
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
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
			}
		}

		[Test]
		public void TestDefaultContextAdd100Entities()
		{
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(SkillTestEntity), new SkillTestEntityElasticsearchMapping());

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.AreEqual(ret.Result.Status, HttpStatusCode.OK);
			}
		}
	}
}
