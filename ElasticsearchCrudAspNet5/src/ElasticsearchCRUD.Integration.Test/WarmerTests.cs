using ElasticsearchCRUD.ContextWarmers;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.Integration.Test
{
    using System;

    using Xunit;

    public class WarmerTests : IDisposable
    {
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		 [Fact]
		public void CreateGlobalWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var warmer = new Warmer("mywarmerone")
				{
					Query = new Query(new MatchAllQuery())
				};
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.WarmerCreate(warmer);
				Assert.True(found);
			}
		}

		 [Fact]
		public void CreateIndexWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var warmer = new Warmer("mywarmertwo")
				{
					Query = new Query(new MatchAllQuery())
				};
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.WarmerCreate(warmer, "existsdtofortestss");
				Assert.True(found);
			}
		}

		 [Fact]
		public void CreateIndexWithTypeWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var warmer = new Warmer("mywarmerthree")
				{
					Query = new Query(new MatchAllQuery())
				};
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.WarmerCreate(warmer, "existsdtofortestss", "wthree");
				Assert.True(found);
			}
		}

		 [Fact]
		[ExpectedException(typeof(ElasticsearchCrudException))]
		public void DeleteGlobalWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				const string warmerName = "mywarmerone";
				var warmer = new Warmer(warmerName)
				{
					Query = new Query(new MatchAllQuery())
				};
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.WarmerCreate(warmer);
				Assert.True(found);

				var ok = context.WarmerDelete(warmerName, "");
				Assert.True(ok);
			}
		}

		 [Fact]
		public void DeleteIndexWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				const string warmerName = "mywarmerone";
				var warmer = new Warmer(warmerName)
				{
					Query = new Query(new MatchAllQuery())
				};
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.WarmerCreate(warmer);
				Assert.True(found);

				var ok = context.WarmerDelete(warmerName, "existsdtofortestss");
				Assert.True(ok);
			}
		}

		 [Fact]
		public void DeleteIndexTypeWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				const string warmerName = "mywarmerthree";
				var warmer = new Warmer(warmerName)
				{
					Query = new Query(new MatchAllQuery())
				};
				context.TraceProvider = new ConsoleTraceProvider();
				var found = context.WarmerCreate(warmer, "existsdtofortestss", "wthree");
				Assert.True(found);

				var ok = context.WarmerDelete(warmerName, "existsdtofortestss");
				Assert.True(ok);
			}
		}

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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
