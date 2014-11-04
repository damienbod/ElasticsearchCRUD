using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class AliasElasticsearchCrudTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException))]
		public void CreateAliasForNoExistingIndex()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex("test", "doesnotexistindex");
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextAlias: index is not allowed in Elasticsearch: doeGGGtindex")]
		public void CreateAliasForIndexBadIndex()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex("test", "doeGGGtindex");
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextAlias: alias is not allowed in Elasticsearch: tesHHHt")]
		public void CreateAliasForIndexBadAlias()
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex("tesHHHt", "doendex");
			}
		}

		[Test]
		public void CreateAliasForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest {Id = 1, Description = "Test index for aliases"};

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var result = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void RemoveAliasForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var resultCreate = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultCreate);

				var resultRemove = context.AliasRemoveForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultRemove);
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException))]
		public void RemoveAliasthatDoesNotExistForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var result = context.AliasRemoveForIndex("tefdfdfdsfst", "indexaliasdtotests");
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void ReplaceIndexForAlias()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };
			var indexAliasDtoTestTwo = new IndexAliasDtoTestTwo { Id = 1, Description = "Test Doc Type Two index for aliases" };

			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.AddUpdateDocument(indexAliasDtoTestTwo, indexAliasDtoTestTwo.Id);
				context.SaveChanges();

				var resultCreate = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultCreate);

				var result = context.AliasReplaceIndex("test", "indexaliasdtotests", "indexaliasdtotesttwos");
				Assert.IsTrue(result);
			}
		}
	}

	public class IndexAliasDtoTest
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}

	public class IndexAliasDtoTestTwo
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}
}
