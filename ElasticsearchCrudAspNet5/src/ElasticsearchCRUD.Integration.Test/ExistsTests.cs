using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.Integration.Test
{
    using System;

    using Xunit;

    public class ExistsTests : IDisposable
    {
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

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
				Assert.True(result);
			}
		}

		 [Fact]
		public void TestIndexExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexExists<ExistsDtoForTests>();
				Assert.True(found);
			}
		}

		 [Fact]
		public void TestIndexTypeExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexTypeExists<ExistsDtoForTests>();
				Assert.True(found);
			}
		}

		 [Fact]
		public void TestIndexDoesNotExist()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexExists<ExistsDtoForTestsIndexNot>();
				Assert.False(found);
			}
		}


		 [Fact]
		public void TestAliasExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExists("existsaliastest");
				Assert.True(found);
			}
		}

		 [Fact]
		public void TestAliasExistsBadAlias()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExists("existsaliastest".ToUpper());
				Assert.False(found);
			}
		}

		 [Fact]
		public void TestAliasExistsForIndex()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExistsForIndex<ExistsDtoForTests>("existsaliastest");
				Assert.True(found);
			}
		}

		 [Fact]
		public void TestIndexExistsForSpecialMapping()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexExists<ExistsDtoForTestsTypeNot>();
				Assert.True(found);
			}
		}

		 [Fact]
		public void TestIndexTypeDoesNotExistsForSpecialMapping()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.IndexTypeExists<ExistsDtoForTestsTypeNot>();
				Assert.False(found);
			}
		}

		 [Fact]
		public void TestAliasNotExists()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExists("noexistsaliastest");
				Assert.False(found);
			}
		}

		 [Fact]
		public void TestAliasNotExistsForIndex()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.AliasExistsForIndex<ExistsDtoForTests>("noexistsaliastest");
				Assert.False(found);
			}
		}

        public void Dispose()
        {
            throw new NotImplementedException();
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
