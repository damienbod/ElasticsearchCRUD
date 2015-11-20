using System;
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
	public class TermsAggregationTests : SetupSearchAgg
	{
		[Fact]
		public void SearchAggTermsBucketAggregationWithNoHits()
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
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggTermsBucketAggregationWithOrderCountNoHits()
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
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				Assert.AreEqual(1, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggTermsBucketAggregationnWithOrderSumSubSumAggNoHits()
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
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				var bucketchildAggSum = aggResult.Buckets[0].GetSingleMetricSubAggregationValue<double>("childAggSum");
				var bucketchildAggAvg = aggResult.Buckets[0].GetSingleMetricSubAggregationValue<double>("childAggAvg");
				Assert.AreEqual(1, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(1.7, bucketchildAggSum);
				Assert.AreEqual(1.7, bucketchildAggAvg);
			}
		}

		[Fact]
		public void SearchAggTermsBucketAggregationnWithOrderSumStatsAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
					{
						Order= new OrderAgg("stats_sub.avg", OrderEnum.desc),
						Aggs = new List<IAggs>
						{
							new StatsMetricAggregation("stats_sub", "lift")
						} 
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				var bucketchildStatsAggregationsResult = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<StatsMetricAggregationsResult>("stats_sub");
				Assert.AreEqual(2, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(2.9, Math.Round(bucketchildStatsAggregationsResult.Avg, 2));
				Assert.AreEqual(5.8, Math.Round(bucketchildStatsAggregationsResult.Sum, 2));
			}
		}

		[Fact]
		public void SearchAggTermsBucketAggregationAndOrderSumSubSumAggNoHits()
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
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				var aggResultAvgMetricAggregation = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("childAgg");
				Assert.AreEqual(1, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(2.33, Math.Round(aggResultAvgMetricAggregation, 2));
			}
		}

		[Fact]
		public void SearchAggTermsBucketAggregationAllPropertiesWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "details")
					{
						CollectMode= CollectMode.breadth_first,
						ExecutionHint= ExecutionHint.global_ordinals,
						MinDocCount = 1,
						Order= new OrderAgg("_term", OrderEnum.desc),
						Include= new IncludeExpression("document*")
						{
							Flags= new List<IncludeExcludeExpressionFlags>
							{
								IncludeExcludeExpressionFlags.CANON_EQ, 
								IncludeExcludeExpressionFlags.CASE_INSENSITIVE,
								IncludeExcludeExpressionFlags.COMMENTS
							}
						},
						Exclude = new ExcludeExpression("nowaytoadthis")
						{
							Flags= new List<IncludeExcludeExpressionFlags>
							{
								IncludeExcludeExpressionFlags.DOTALL, 
								IncludeExcludeExpressionFlags.LITERAL,
								IncludeExcludeExpressionFlags.MULTILINE
							}
						}

					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				Assert.AreEqual(2, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggTermsBucketAggregationScriptWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "lift")
					{
						Script = "_value * times * constant",
						Params = new List<ScriptParameter>
						{
							new ScriptParameter("times", 1.4),
							new ScriptParameter("constant", 10.2)
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}

	}
}
