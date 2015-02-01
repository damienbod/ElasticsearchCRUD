using System;
using System.Collections.Generic;
using System.Globalization;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class TermsAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggMinAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleValueMetric<double>("test_min");
				Assert.AreEqual("2.1", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggMinAggregationWithOrderCountNoHits()
		{
			var search = new Search 
			{ 
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
					{
						Order= new OrderAgg("_term", OrderEnum.asc)
					}
				} 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleValueMetric<double>("test_min");
				Assert.AreEqual("2.1", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggMinAggregationWithOrderSumSubSumAggNoHits()
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
							new AvgMetricAggregation("childAggAvg", "lift")
						} 
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleValueMetric<double>("test_min");
				Assert.AreEqual("2.1", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggMinAggregationAndOrderSumSubSumAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
					{
						Order= new OrderAgg("_term", OrderEnum.asc)
					},
					new AvgMetricAggregation("childAgg", "lift")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleValueMetric<double>("test_min");
				Assert.AreEqual("2.1", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}
