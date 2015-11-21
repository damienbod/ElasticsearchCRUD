using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	public class SearchQueryQuerySpanTests : SetupSearch, IDisposable
    {
        public SearchQueryQuerySpanTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
		public void SearchQuerySpanFirstQueryWithSpanTermQuery()
		{
			var search = new Search
			{
				Query = new Query(new SpanFirstQuery(new SpanTermQuery("name", "one"), 3))
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(3, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
		public void SearchQuerySpanNotQueryPostPre()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanNotQuery(
						new SpanTermQuery("name", "one"),
						new SpanTermQuery("name", "bvobx") { Boost = 1.9 }			
					)
					{
						Pre = 2,
						Post = 6
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
		public void SearchQuerySpanNotQueryDist()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanNotQuery(
						new SpanTermQuery("name", "one"),
						new SpanTermQuery("name", "bvobx") { Boost = 1.9 }
					)
					{
						Dist = 2
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}

		[Fact]
		public void SearchQuerySpanNearQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new SpanNearQuery(new List<SpanTermQuery>
					{
						new SpanTermQuery("details", "document"),
						new SpanTermQuery("details", "two") {Boost = 1.9}
					}, 50)
					{
						CollectPayloads=true,
						InOrder=true
					})
						
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.Total);
			}
		}
	}

}