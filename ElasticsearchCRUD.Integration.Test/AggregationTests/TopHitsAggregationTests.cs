using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class TopHitsAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggTopHitsMetricAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TopHitsMetricAggregation("topHits")
					{
						Size = 5
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsAggregationsResult>("test_min");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}

		[Test]
		public void SearchAggTermsBucketAggregationWithSubTopHitsMetricAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
					{
						Aggs = new List<IAggs>
						{
							new TopHitsMetricAggregation("topHits")
							{
								Size = 5
							}
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsAggregationsResult>("test_min");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}
	}
}
