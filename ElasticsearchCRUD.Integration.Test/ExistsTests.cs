using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class ExistsTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<ExistsDtoForTests>();
				entityResult.Wait();
			}
		}

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			var existsDtoForTests = new ExistsDtoForTests { Id = 1, Description = "Test index for exist tests" };
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ExistsDtoForTestsTypeNot), new IndexMapping("existsdtofortestss"));

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(existsDtoForTests, existsDtoForTests.Id);
				context.SaveChanges();

				var result = context.AliasCreateForIndex("existsaliastest", "existsdtofortestss");
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void TestIndexExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexExists<ExistsDtoForTests>();
				Assert.IsTrue(found);
			}
		}

		[Test]
		public void TestIndexTypeExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexTypeExists<ExistsDtoForTests>();
				Assert.IsTrue(found);
			}
		}

		[Test]
		public void TestIndexDoesNotExist()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexExists<ExistsDtoForTestsIndexNot>();
				Assert.IsFalse(found);
			}
		}


		[Test]
		public void TestAliasExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExists("existsaliastest");
				Assert.IsTrue(found);
			}
		}

		[Test]
		public void TestAliasExistsBadAlias()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExists("existsaliastest".ToUpper());
				Assert.IsFalse(found);
			}
		}

		[Test]
		public void TestAliasExistsForIndex()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExistsForIndex<ExistsDtoForTests>("existsaliastest");
				Assert.IsTrue(found);
			}
		}

		[Test]
		public void TestIndexExistsForSpecialMapping()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexExists<ExistsDtoForTestsTypeNot>();
				Assert.IsTrue(found);
			}
		}

		[Test]
		public void TestIndexTypeDoesNotExistsForSpecialMapping()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexTypeExists<ExistsDtoForTestsTypeNot>();
				Assert.IsFalse(found);
			}
		}

		[Test]
		public void TestAliasNotExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExists("noexistsaliastest");
				Assert.IsFalse(found);
			}
		}

		[Test]
		public void TestAliasNotExistsForIndex()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExistsForIndex<ExistsDtoForTests>("noexistsaliastest");
				Assert.IsFalse(found);
			}
		}

	}

	public class ExistsDtoForTests
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}

	public class ExistsDtoForTestsIndexNot
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}

	public class ExistsDtoForTestsTypeNot
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}
}
