using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryQueryCommonTermsTests : SetupSearch
	{
		[Test]
		public void SearchQueryCommonTermsQueryLikeBoolMatch()
		{
			var search = new Search
			{
				Query = new Query(
					new CommonTermsQuery("details", "leave it alone", 0.001)
					{
						LowFreqOperator = QueryDefaultOperator.AND
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
		public void SearchQueryCommonTermsQueryLikeBoolMatchTwo()
		{
			var search = new Search
			{
				Query = new Query(
					new CommonTermsQuery("details", "leave it alone", 0.001)
					{
						LowFreqOperator = QueryDefaultOperator.AND,
						MinimumShouldMatch = 2
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
		public void SearchQueryCommonTermsQueryLikeBoolMatchHighLowMin()
		{
			var search = new Search
			{
				Query = new Query(
					new CommonTermsQuery("details", "leave it alone", 0.001)
					{
						LowFreqOperator = QueryDefaultOperator.AND,
						LowFreq = 1,
						HighFreq = 3
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

	}
}