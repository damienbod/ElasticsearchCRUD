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
	public class GlobalBucketAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggGlobalBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new GlobalBucketAggregation("globalbucket")				
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GlobalBucketAggregationsResult>("globalbucket");
				Assert.AreEqual(7, aggResult.DocCount);
			}
		}

		[Test]
		public void SearchAggGlobalBucketAggregationWithChildrenNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new GlobalBucketAggregation("globalbucket")
					{
						Aggs = new List<IAggs>
						{
							new MaxMetricAggregation("maxAgg", "lift"),
							new MinMetricAggregation("minAgg", "lift"),
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GlobalBucketAggregationsResult>("globalbucket");
				var max = aggResult.GetSingleMetricSubAggregationValue<double>("maxAgg");
				Assert.AreEqual(7, aggResult.DocCount);
				Assert.AreEqual(2.9, max);
			}
		}

		[Test]
		[ExpectedException(ExpectedMessage = "GlobalBucketAggregation cannot be sub aggregations")]
		public void SearchAggTermsBucketAggregationWithOrderSumSubSumAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
					{
						Order= new OrderAgg("childAggSum", OrderEnum.asc),
						Aggs = new List<IAggs>
						{
							new SumMetricAggregation("childAggSum", "lift"),
							new AvgMetricAggregation("childAggAvg", "lift"),
							new GlobalBucketAggregation("test")
						} 
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				var bucketchildAggSum = aggResult.Buckets[0].GetSingleMetricSubAggregationValue<double>("childAggSum");
				var bucketchildAggAvg = aggResult.Buckets[0].GetSingleMetricSubAggregationValue<double>("childAggAvg");
				Assert.AreEqual(1, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(1.7, bucketchildAggSum);
				Assert.AreEqual(1.7, bucketchildAggAvg);
			}
		}

	}
}
