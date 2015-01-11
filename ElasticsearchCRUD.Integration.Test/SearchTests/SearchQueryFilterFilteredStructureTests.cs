using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
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
			var search = new Search { Query = new Query(new MatchAllQuery(){ Boost = 1.1}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilterMatchAllTest()
		{
			var search = new Search { Filter = new Filter(new MatchAllFilter{ Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchTest()
		{
			var search = new Search { Query = new Query(new MatchQuery("name", "document one"){Boost=1.1}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchPhaseTest()
		{
			var search = new Search { Query = new Query(new MatchPhaseQuery("name", "one") { Analyzer = LanguageAnalyzers.German, Slop = 1, Operator = Operator.and, CutoffFrequency = 0.2, ZeroTermsQuery = ZeroTermsQuery.none, Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchPhasePrefixTest()
		{
			var search = new Search { Query = new Query(new MatchPhasePrefixQuery("name", "one") { MaxExpansions = 500, Boost = 1.1, MinimumShouldMatch = "50%" }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
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
							new MatchAllFilter { Boost = 1.1 }
						)
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilteredQueryFilterMatchAllQueryMatchAll()
		{
			var search = new Search
			{
				Query = new Query(
					new Filtered( 
						new Filter( new MatchAllFilter { Boost = 1.1 } )) { Query = new Query(new MatchAllQuery())}		
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMultiMatchAllTest()
		{
			var search = new Search { Query = new Query(new MultiMatch("document"){MultiMatchType= MultiMatchType.most_fields, TieBreaker = 0.5, Fields = new List<string>{"name", "details"}}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermQuery()
		{
			var search = new Search { Query = new Query(new TermQuery("name", "one"){Boost=2.0}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsQuery()
		{
			var search = new Search { Query = new Query(new TermsQuery("name", new List<string>{"one"}){ Boost = 2.0 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsQueryTwoResults()
		{
			var search = new Search { Query = new Query(new TermsQuery("name", new List<string> { "one", "two" }) { Boost = 2.0, MinimumShouldMatch="1" }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[1].Source.Id);
			}
		}

		[Test]
		public void SearchQueryRangeQuery()
		{
			var search = new Search { Query = new Query(new RangeQuery("id"){GreaterThanOrEqualTo="2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1",Boost=2.0}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermFilter()
		{
			var search = new Search { Filter = new Filter(new TermFilter("name", "three"){Cache=false})};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsFilter()
		{
			var search = new Search { Filter = new Filter(new TermsFilter("name", new List<string>{"one", "three"}) { Cache = false, Execution=ExecutionMode.@bool }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[1].Source.Id);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsFilterExecutionModeAnd()
		{
			var search = new Search { Filter = new Filter(new TermsFilter("name", new List<string> { "one", "three" }) { Cache = false, Execution = ExecutionMode.and }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(0, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryRangeFilter()
		{
			var search = new Search { Filter = new Filter(new RangeFilter("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1", Cache = false}) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider= new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}


		[TestFixtureTearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				//var entityResult = context.DeleteIndexAsync<SearchTest>(); entityResult.Wait();
			}
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			var doc1 = new SearchTest
			{
				Id = 1,
				Details = "This is the details of the document, very interesting",
				Name = "one",
				CircleTest = new GeoShapeCircle() { Radius = "100m", Coordinates = new GeoPoint(45, 45) }
			};

			var doc2 = new SearchTest
			{
				Id = 2,
				Details = "Details of the document two, leave it alone",
				Name = "two",
				CircleTest = new GeoShapeCircle() { Radius = "50m", Coordinates = new GeoPoint(46, 45) }
			};
			var doc3 = new SearchTest
			{
				Id = 3,
				Details = "This data is different",
				Name = "three",
				CircleTest = new GeoShapeCircle() { Radius = "80m", Coordinates = new GeoPoint(37, 42) }
			};
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				//context.IndexCreate<SearchTest>();
				Thread.Sleep(1000);
				context.AddUpdateDocument(doc1, doc1.Id);
				context.AddUpdateDocument(doc2, doc2.Id);
				context.AddUpdateDocument(doc3, doc3.Id);
				context.SaveChanges();
				Thread.Sleep(1000);
			}
		}
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
