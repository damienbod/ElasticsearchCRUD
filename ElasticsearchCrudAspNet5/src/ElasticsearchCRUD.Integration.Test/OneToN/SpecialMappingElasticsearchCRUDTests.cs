using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.OneToN
{
    using Xunit;

    [TestFixture]
	public class SpecialMappingElasticsearchCrudTests
	{
		private List<SkillTestEntity> _entitiesForTests;
		private List<SkillTestEntityTwo> _entitiesForTestsTypeTwo;
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

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
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

				entityResult.Wait();
				var secondDelete = context.DeleteIndexAsync<SkillTestEntityTwo>();
				secondDelete.Wait();
			}
		}

		[Fact]
		public void TestDefaultContextAdd100Entities()
		{
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(SkillTestEntity), new SkillTestEntityElasticsearchMapping());

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				for (int i = 0; i < 100; i++)
				{
					context.AddUpdateDocument(_entitiesForTests[i], i);
				}

				// Save to Elasticsearch
				var ret = context.SaveChangesAsync();
				Assert.Equal(ret.Result.Status, HttpStatusCode.OK);
			}
		}
	}
}
