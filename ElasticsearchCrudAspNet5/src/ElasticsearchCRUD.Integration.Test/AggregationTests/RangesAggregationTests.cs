using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class RangessAggregationTests : SetupSearchAgg, IDisposable
    {
        public RangessAggregationTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
        public void SearchAggRangesBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new RangeBucketAggregation<double>("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter<double>>
                    {
                        new ToRangeAggregationParameter<double>(1.5),
                        new ToFromRangeAggregationParameter<double>(1.5,2.0),
                        new FromRangeAggregationParameter<double>(2.0)
                    })
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
                Assert.Equal(6, aggResult.Buckets[2].DocCount);
                Assert.Equal("2.0", aggResult.Buckets[2].FromAsString);
            }
        }

        [Fact]
        public void SearchAggRangesBucketAggregationKeyedWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new RangeBucketAggregation<double>("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter<double>>
                    {
                        new ToRangeAggregationParameter<double>(1.5),
                        new ToFromRangeAggregationParameter<double>(1.5,2.0),
                        new FromRangeAggregationParameter<double>(2.0)
                    })
                    {
                        Keyed= true
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
                var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("2.0-*");

                Assert.Equal("2.0", test.FromAsString);
            }
        }

        [Fact]
        public void SearchAggRangesBucketAggregationWithRangeKeysWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new RangeBucketAggregation<double>("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter<double>>
                    {
                        new ToRangeAggregationParameter<double>(1.5)
                        {
                            Key = "one"
                        },
                        new ToFromRangeAggregationParameter<double>(1.5,2.0)
                        {
                            Key = "two"
                        },
                        new FromRangeAggregationParameter<double>(2.0)
                        {
                            Key = "three"
                        }
                    })
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
                Assert.Equal(6, aggResult.Buckets[2].DocCount);
                Assert.Equal("three", aggResult.Buckets[2].Key);
            }
        }

        [Fact]
        public void SearchAggRangesBucketAggregationKeyedWithRangeKeysWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new RangeBucketAggregation<double>("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter<double>>
                    {
                        new ToRangeAggregationParameter<double>(1.5)
                        {
                            Key = "one"
                        },
                        new ToFromRangeAggregationParameter<double>(1.5,2.0)
                        {
                            Key = "two"
                        },
                        new FromRangeAggregationParameter<double>(2.0)
                        {
                            Key = "three"
                        }
                    })
                    {
                        Keyed= true
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
                var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("three");

                Assert.Equal("2.0", test.FromAsString);
            }
        }

        [Fact]
        public void SearchAggRangesBucketAggregationWithRangeKeysWithMaxMetricSubAggWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new RangeBucketAggregation<double>("testRangesBucketAggregation", "lift", new List<RangeAggregationParameter<double>>
                    {
                        new ToRangeAggregationParameter<double>(1.5)
                        {
                            Key = "one"
                        },
                        new ToFromRangeAggregationParameter<double>(1.5,2.0)
                        {
                            Key = "two"
                        },
                        new FromRangeAggregationParameter<double>(2.0)
                        {
                            Key = "three"
                        }
                    })
                    {
                        Aggs = new List<IAggs>
                        {
                            new MaxMetricAggregation("maxi", "lift")
                        }
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
                var max = aggResult.Buckets[2].GetSingleMetricSubAggregationValue<double>("maxi");
                Assert.Equal(6, aggResult.Buckets[2].DocCount);
                Assert.Equal("three", aggResult.Buckets[2].Key);
                Assert.Equal(2.9, max);
            }
        }

        [Fact]
        public void SearchAggDateRangesBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new DateRangeBucketAggregation("testRangesBucketAggregation", "dateofdetails", "MM-yyy", new List<RangeAggregationParameter<string>>
                    {
                        new ToRangeAggregationParameter<string>("now-8M/M"),
                        new ToFromRangeAggregationParameter<string>("now-8M/M", "now-10M/M"),
                        new FromRangeAggregationParameter<string>("now-10M/M")
                    })
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
                Assert.Equal(7, aggResult.Buckets[2].DocCount);
            }
        }

        [Fact]
        public void SearchAggDateRangesBucketAggregationKeyedWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new DateRangeBucketAggregation("testRangesBucketAggregation", "dateofdetails", "MM-yyy", new List<RangeAggregationParameter<string>>
                    {
                        new ToRangeAggregationParameter<string>("now-8M/M"),
                        new ToFromRangeAggregationParameter<string>("now-8M/M", "now-10M/M"),
                        new FromRangeAggregationParameter<string>("now-10M/M")
                        {
                            Key = "keyName"
                        }
                    })
                    {
                        Keyed= true
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
                var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("keyName");

                Assert.Equal(7, test.DocCount);			
            }
        }

        [Fact]
        public void SearchAggGeoDistanceBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeoDistanceBucketAggregation("testGeoDistanceBucketAggregation", "location", new GeoPoint(40.0, 40.0), 
                        new List<RangeAggregationParameter<uint>>
                        {
                            new ToRangeAggregationParameter<uint>(100),
                            new ToFromRangeAggregationParameter<uint>(100, 500),
                            new FromRangeAggregationParameter<uint>(500)
                        })
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoDistanceBucketAggregationsResult>("testGeoDistanceBucketAggregation");
                Assert.Equal(7, aggResult.Buckets[2].DocCount);
                Assert.Equal<uint>(500, aggResult.Buckets[2].From);
            }
        }

        [Fact]
        public void SearchAggGeoDistanceBucketAggregationWithDistanceTypeWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeoDistanceBucketAggregation("testGeoDistanceBucketAggregation", "location", new GeoPoint(40.0, 40.0), 
                        new List<RangeAggregationParameter<uint>>
                        {
                            new ToRangeAggregationParameter<uint>(100),
                            new ToFromRangeAggregationParameter<uint>(100, 500),
                            new FromRangeAggregationParameter<uint>(500)
                        })
                    {
                        DistanceType = DistanceType.plane,
                        Unit = DistanceUnitEnum.m
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoDistanceBucketAggregationsResult>("testGeoDistanceBucketAggregation");
                Assert.Equal(7, aggResult.Buckets[2].DocCount);
                Assert.Equal<uint>(500, aggResult.Buckets[2].From);
            }
        }

        [Fact]
        public void SearchAggGeoDistanceBucketAggregationWithDistanceTypeWithTopHitsWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeoDistanceBucketAggregation("testGeoDistanceBucketAggregation", "location", new GeoPoint(40.0, 40.0), 
                        new List<RangeAggregationParameter<uint>>
                        {
                            new ToRangeAggregationParameter<uint>(100),
                            new ToFromRangeAggregationParameter<uint>(100, 500),
                            new FromRangeAggregationParameter<uint>(500)
                        })
                    {
                        DistanceType = DistanceType.plane,
                        Unit = DistanceUnitEnum.m,
                        Aggs = new List<IAggs>
                        {
                            new TopHitsMetricAggregation("tops")
                        }
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoDistanceBucketAggregationsResult>("testGeoDistanceBucketAggregation");
                var hits = aggResult.Buckets[2].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("tops");
                Assert.Equal(7, aggResult.Buckets[2].DocCount);
                Assert.Equal<uint>(500, aggResult.Buckets[2].From);
                Assert.Equal(7, hits.Hits.Total);
            }
        }
    }
}
