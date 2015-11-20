using System;
using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    using Xunit;

    public class HistogramBucketAggregationTests : SetupSearchAgg, IDisposable
    {
		[Fact]
		public void SearchAggHistogramBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("testHistogramBucketAggregation", "lift", 1)
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("testHistogramBucketAggregation");
				Assert.Equal(1, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationWithOrderCountNoHits()
		{
			var search = new Search 
			{ 
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("test_min", "lift", 10)
					{
						Order= new OrderAgg("_term", OrderEnum.asc)
					}
				} 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("test_min");
				Assert.Equal(7, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationWithOrderSumSubSumAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("test_min", "lift", 1)
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("test_min");
				var bucketchildAggSum = aggResult.Buckets[0].GetSingleMetricSubAggregationValue<double>("childAggSum");
				var bucketchildAggAvg = aggResult.Buckets[0].GetSingleMetricSubAggregationValue<double>("childAggAvg");
				Assert.Equal(1, aggResult.Buckets[0].DocCount);
				Assert.Equal(1.7, bucketchildAggSum);
				Assert.Equal(1.7, bucketchildAggAvg);
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationWithOrderSumStatsAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("test_min", "lift", 1)
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("test_min");
				var bucketchildStatsAggregationsResult = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<StatsMetricAggregationsResult>("stats_sub");
				Assert.Equal(6, aggResult.Buckets[0].DocCount);
				Assert.Equal(2.43, Math.Round(bucketchildStatsAggregationsResult.Avg, 2));
				Assert.Equal(14.6, Math.Round(bucketchildStatsAggregationsResult.Sum, 2));
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationnAndOrderSumSubSumAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("test_min", "lift", 1)
					{
						Order= new OrderAgg("_count", OrderEnum.asc)
					},
					new AvgMetricAggregation("childAgg", "lift")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("test_min");
				var aggResultAvgMetricAggregation = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("childAgg");
				Assert.Equal(1, aggResult.Buckets[0].DocCount);
				Assert.Equal(2.33, Math.Round(aggResultAvgMetricAggregation, 2));
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationAllPropertiesWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("test_min", "lift", 1)
					{
						MinDocCount = 1,
						Order= new OrderAgg("_count", OrderEnum.desc),
						ExtendedBounds= new ExtendedBounds{Max=100, Min=0},
						Keyed=false
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("test_min");
				Assert.Equal(6, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationScriptWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("test_min", "lift", 1)
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramBucketAggregationsResult>("test_min");
				Assert.Equal(1, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggHistogramBucketAggregationKeyedWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new HistogramBucketAggregation("testHistogramBucketAggregation", "lift", 1)
					{
						Keyed = true
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<HistogramNamedBucketAggregationsResult>("testHistogramBucketAggregation");
				Assert.Equal(1, aggResult.Buckets.GetSubAggregationsFromJTokenName<Bucket>("1").DocCount);
			}
		}

		[Fact]
		public void SearchAggDateHistogramBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new DateHistogramBucketAggregation("testHistogramBucketAggregation", "dateofdetails", new TimeUnitMonth(1))
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<DateHistogramBucketAggregationsResult>("testHistogramBucketAggregation");
				Assert.Equal(1, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggDateHistogramBucketAggregationWithTopHitsAggWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new DateHistogramBucketAggregation("testHistogramBucketAggregation", "dateofdetails", new TimeUnitMonth(1))
					{
						Aggs =  new List<IAggs>
						{
							new TopHitsMetricAggregation("tophits")
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<DateHistogramBucketAggregationsResult>("testHistogramBucketAggregation");
				var tophits = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("tophits");
				Assert.Equal(1, aggResult.Buckets[0].DocCount);
				Assert.Equal(1, tophits.Hits.Total);
			}
		}

		[Fact]
		public void SearchAggDateHistogramBucketAggregationWithPropertiesWithTopHitsAggWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new DateHistogramBucketAggregation("testHistogramBucketAggregation", "dateofdetails", new TimeUnitMonth(1))
					{
						Aggs =  new List<IAggs>
						{
							new TopHitsMetricAggregation("tophits")
						},
						MinDocCount=2,
						//Order= new OrderAgg("dateofdetails", OrderEnum.desc), // TODO Order not working in EL 2.0 ???
						ExtendedBounds= new ExtendedBounds
						{
							Max= 1000000,
							Min = 0
						},
						Offset= "1d",
						Format = "yyyy-MM-dd",
						TimeZone = "+02:00"
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<DateHistogramBucketAggregationsResult>("testHistogramBucketAggregation");
				var tophits = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("tophits");
				Assert.Equal(3, aggResult.Buckets[0].DocCount);
				Assert.Equal(3, tophits.Hits.Total);
			}
		}

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
