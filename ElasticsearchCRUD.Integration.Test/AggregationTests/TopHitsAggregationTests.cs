using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
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
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TopHitsAggregationsResult<SearchAggTest>>("topHits");
				Assert.AreEqual(7, aggResult.Hits.Total);
				Assert.AreEqual(2.1, aggResult.Hits.HitsResult[0].Source.Lift);
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
				var hitsForBucket = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsAggregationsResult<SearchAggTest>>("topHits");
				
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(2.1, hitsForBucket.Hits.HitsResult[0].Source.Lift);
			}
		}


		[Test]
		public void SearchAggTermsBucketAggregationWithSubTopHitsMetricAggregationSortWithNoHits()
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
								Size = 5,
								Sort = new SortHolder(
									new List<ISort>
									{
										new SortStandard("lift")
										{
											Order = OrderEnum.desc
										},
										new SortStandard("lengthofsomething")
										{
											Order = OrderEnum.asc
										}
									}
								),
								From= 1
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
				var hitsForBucket = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsAggregationsResult<SearchAggTest>>("topHits");

				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(2.1, hitsForBucket.Hits.HitsResult[0].Source.Lift);
			}
		}
	}
}
