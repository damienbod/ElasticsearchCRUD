using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class GeohashGridBucketAggregationTests : SetupSearchAgg, IDisposable
    {
        public GeohashGridBucketAggregationTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
        public void SearchAggGeohashGridBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeohashGridBucketAggregation("testGeohashGridBucketAggregation", "location")
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
                Assert.Equal(3, aggResult.Buckets[0].DocCount);
            }
        }

        [Fact]
        public void SearchAggGeohashGridBucketAggregationWithTopHitsSubWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeohashGridBucketAggregation("testGeohashGridBucketAggregation", "location")
                    {
                        Aggs = new List<IAggs>
                        {
                            new TopHitsMetricAggregation("tophits")
                        }
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
                var topHits = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("tophits");
                Assert.Equal(3, aggResult.Buckets[0].DocCount);
                Assert.Equal(3, topHits.Hits.Total);
            }
        }

        [Fact]
        public void SearchAggGeohashGridBucketAggregationPrecisionWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeohashGridBucketAggregation("testGeohashGridBucketAggregation", "location")
                    {
                        Precision = 7,
                        Size = 100,
                        ShardSize= 200
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
                Assert.Equal(3, aggResult.Buckets[0].DocCount);
            }
        }

        [Fact]
        public void SearchAggTermsBucketAggregationScriptWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeohashGridBucketAggregation("testGeohashGridBucketAggregation", "location")
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
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
                Assert.Equal(3, aggResult.Buckets[0].DocCount);
            }
        }
    }
}
