using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class TopHitsAggregationTests : SetupSearchAgg, IDisposable
    {
        public TopHitsAggregationTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
        public void SearchAggTopHitsMetricAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
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
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TopHitsMetricAggregationsResult<SearchAggTest>>("topHits");
                Assert.Equal(7, aggResult.Hits.Total);
                Assert.Equal(1.7, aggResult.Hits.HitsResult[0].Source.Lift);
            }
        }

        [Fact]
        public void SearchAggTermsBucketAggregationWithSubTopHitsMetricAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
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
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
                var hitsForBucket = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("topHits");
                
                Assert.Equal(3, aggResult.Buckets[0].DocCount);
                Assert.Equal(2.1, hitsForBucket.Hits.HitsResult[0].Source.Lift);
            }
        }

        [Fact]
        public void SearchAggTermsBucketAggregationWithSubTopHitsMetricAggregationSortWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
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
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");
                var hitsForBucket = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("topHits");

                Assert.Equal(3, aggResult.Buckets[0].DocCount);
                Assert.Equal(2.1, hitsForBucket.Hits.HitsResult[0].Source.Lift);
            }
        }
    }
}
