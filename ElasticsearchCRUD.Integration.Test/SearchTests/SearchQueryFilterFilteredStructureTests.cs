using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.SearchModel.QueryAndFilter;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryFilterFilteredStructureTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[Test]
		public void SearchQueryMatchAllTest()
		{
			var search = new Search { Query = new Query(new MatchAll{ Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		[Test]
		public void SearchFilterMatchAllTest()
		{
			var search = new Search { Filter = new Filter(new MatchAll { Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		[Test]
		public void SearchQueryMatchTest()
		{
			var search = new Search { Query = new Query(new Match("name", "document one"){Boost=1.1}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		[Test]
		public void SearchQueryMatchPhaseTest()
		{
			var search = new Search { Query = new Query(new MatchPhase("name", "document one") { Analyzer = LanguageAnalyzers.German, Slop = 1, Operator = Operator.and, CutoffFrequency = 0.2, ZeroTermsQuery = ZeroTermsQuery.none, Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		[Test]
		public void SearchQueryMatchPhasePrefixTest()
		{
			var search = new Search { Query = new Query(new MatchPhasePrefix("name", "document one") { MaxExpansions = 500, Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		[Test]
		public void SearchFilteredQueryFilterMatchAll()
		{
			var search = new Search
			{
				Query = new Query(
					new Filtered(
						new Filter(
							new MatchAll { Boost = 1.1 }
						)
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		[Test]
		public void SearchFilteredQueryFilterMatchAllQueryMatchAll()
		{
			var search = new Search
			{
				Query = new Query(
					new Filtered( new Filter( new MatchAll { Boost = 1.1 } )) { Query = new Query(new MatchAll())}		
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
			}
		}

		//[TestFixtureTearDown]
		//public void TearDown()
		//{
		//	using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
		//	{
		//		context.AllowDeleteForIndex = true;
		//		var entityResult = context.DeleteIndexAsync<SearchTest>(); entityResult.Wait();
		//	}
		//}

		//[TestFixtureSetUp]
		//public void Setup()
		//{
		//	var doc1 = new SearchTest
		//	{
		//		Id = 1,
		//		Details = "These a the details",
		//		Name = "document one",
		//		CircleTest = new GeoShapeCircle() { Radius = "100m", Coordinates = new GeoPoint(45, 45) }
		//	};
		//	using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
		//	{
		//		context.IndexCreate<SearchTest>();
		//		context.AddUpdateDocument(doc1, doc1.Id);
		//		context.SaveChanges();
		//	}
		//}
	}

	public class SearchTest
	{
		public long Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchString(Analyzer = LanguageAnalyzers.German)]
		public string Details { get; set; }

		[ElasticsearchGeoShape]
		public GeoShapeCircle CircleTest { get; set; }
	}
}
