using ElasticsearchCRUD.ContextWarmers;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class WarmerTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[Test]
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
				Assert.IsTrue(found);
			}
		}

		[Test]
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
				Assert.IsTrue(found);
			}
		}

		[Test]
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
				Assert.IsTrue(found);
			}
		}

		[Test]
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
				Assert.IsTrue(found);

				var ok = context.WarmerDelete(warmerName);
				Assert.IsTrue(ok);
			}
		}

		[Test]
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
				Assert.IsTrue(found);

				var ok = context.WarmerDelete(warmerName, "existsdtofortestss");
				Assert.IsTrue(ok);
			}
		}

		[Test]
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
				Assert.IsTrue(found);

				var ok = context.WarmerDelete(warmerName, "existsdtofortestss");
				Assert.IsTrue(ok);
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
				Assert.IsTrue(result);
			}
		}

	}
}
