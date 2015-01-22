using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryQuerySpanTests : SetupSearch
	{
		[Test]
		public void SearchQuerySpanFirstQueryWithSpanTermQuery()
		{
			var search = new Search
			{
				Query = new Query(new SpanFirstQuery(new SpanTermQuery("name", "one"), 3))
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
		public void SearchQuerySpanTermQuery()
		{
			var search = new Search
			{
				Query = new Query(new SpanTermQuery("name", "one")
				{
					Boost = 1.9
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

		[Test]
		public void SearchQuerySpanFirstQueryWithSpanMultiQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanFirstQuery(
						new SpanMultiQuery(
							new PrefixQuery("name", "on")
						), 3))
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
		public void SearchQuerySpanMultiQueryWithPrefixQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanMultiQuery(
						new PrefixQuery("name", "on"))
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
		public void SearchQuerySpanMultiQueryWithFuzzyQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanMultiQuery(
						new FuzzyQuery("details", "dsta"))
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
		public void SearchQuerySpanMultiQueryWithRegExpQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanMultiQuery(
						new RegExpQuery("name", "o.*"))
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
		public void SearchQuerySpanMultiQueryWithRangeQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanMultiQuery(
						new RangeQuery("details")
						{
							GreaterThanOrEqualTo = "one"
						})
					)
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
		public void SearchQuerySpanMultiQueryWithWildcardQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanMultiQuery(
						new WildcardQuery("details", "t*o"))
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
		public void SearchQuerySpanOrQuery()
		{
			var search = new Search
			{
				Query = new Query(new SpanOrQuery(
					new List<ISpanQuery>()
					{
						new SpanTermQuery("name", "one") { Boost = 1.9 },
						new SpanMultiQuery( new PrefixQuery("name", "on"))
					
					}))
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