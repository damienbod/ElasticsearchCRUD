using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryFilterTests : SetupSearch
	{
		[Test]
		public void SearchFilterMatchAllTest()
		{
			var search = new Search { Filter = new Filter(new MatchAllFilter { Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermFilter()
		{
			var search = new Search { Filter = new Filter(new TermFilter("name", "three") { Cache = false }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsFilter()
		{
			var search = new Search { Filter = new Filter(new TermsFilter("name", new List<object> { "one", "three" }) { Cache = false, Execution = ExecutionMode.@bool }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
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
			var search = new Search { Filter = new Filter(new TermsFilter("name", new List<object> { "one", "three" }) { Cache = false, Execution = ExecutionMode.and }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(0, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryRangeFilter()
		{
			var search = new Search { Filter = new Filter(new RangeFilter("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1" }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryBoolFilter()
		{
			var search = new Search
			{
				Filter = new Filter(
					new BoolFilter( new RangeFilter("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1" })
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryBoolFilterTwo()
		{
			var search = new Search
			{
				Filter = new Filter(
					new BoolFilter
					{
						Must = new List<IFilter>
						{
							new RangeFilter("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1" }
						},
						MustNot = new List<IFilter>
						{
							new RangeFilter("id") {GreaterThan="34"}
						},
						Cache = false,
						Should = new List<IFilter>
						{
							new TermFilter("name", "two")
						}
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryExistsFilter()
		{
			var search = new Search { Filter = new Filter(new ExistsFilter("name")) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryAndFilter()
		{
			var search = new Search { Filter = new Filter(
					new AndFilter
					{
						And = new List<IFilter>
						{
							new ExistsFilter("name"),
							new TermFilter("name", "one")
						},
						Cache = false
					}
				) 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryOrFilter()
		{
			var search = new Search
			{
				Filter = new Filter(
					new OrFilter
					{
						Or = new List<IFilter>
						{
							new TermFilter("name", "one"),
							new TermFilter("name", "two")
						},
						Cache = false
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryNotFilter()
		{
			var search = new Search
			{
				Filter = new Filter(
					new NotFilter(new TermFilter("name", "one"))
					{
						Cache = false
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryGeoDistanceFilter()
		{
			var search = new Search { Filter = new Filter(new GeoDistanceFilter("location", new GeoPoint(43,43), new DistanceUnitKilometer(1000) )) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryGeoDistanceFilterSmallRange()
		{
			var search = new Search { Filter = new Filter(new GeoDistanceFilter("location", new GeoPoint(43, 43), new DistanceUnitKilometer(1))
			{
				Cache= false,
				DistanceType= DistanceType.plane,
				OptimizeBbox = OptimizeBbox.none
			}) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(0, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryIdsFilter()
		{
			var search = new Search
			{
				Filter = new Filter(new IdsFilter(new List<object> { 1, 2 }, "searchtest"))
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchFilteredQueryFilterLimit()
		{
			var search = new Search
			{
				Query = new Query(
					new Filtered(
						new Filter(
							new LimitFilter(1))
						)
					)
				
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryQueryFilter()
		{
			var search = new Search
			{
				Filter = new Filter(new QueryFilter(new MatchAllQuery())
				{
					Cache=true
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryPrefixFilter()
		{
			var search = new Search
			{
				Filter = new Filter(new PrefixFilter("name", "on")
				{
					Cache = false
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.Total);
			}
		}
	}
}