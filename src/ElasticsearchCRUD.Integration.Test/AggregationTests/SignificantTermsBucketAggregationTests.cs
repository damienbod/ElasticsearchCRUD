using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class SignificantTermsBucketAggregationTests : SetupSearchAgg, IDisposable
    {
        public SignificantTermsBucketAggregationTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
        public void SearchAggSignificantTermsBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
                Assert.Equal(7, aggResult.DocCount);
            }
        }

        [Fact]
        public void SearchAggTermsBucketAggregationWithSubAggSignificantTermsBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new TermsBucketAggregation("termsDetails", "details")
                    {
                        Aggs = new List<IAggs>
                        {
                            new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
                            {
                                InformationRetrievalRetrieval = new Jlh()
                            }
                        }
                    }					
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var termsAggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("termsDetails");
                var significantAggResult = termsAggResult.Buckets[0].GetSubAggregationsFromJTokenName<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
                Assert.Equal(4, significantAggResult.Buckets[0].BgCount);
            }
        }

        [Fact]
        public void SearchAggSignificantTermsBucketAggregationPropertiesSet()
        {
            var search = new Search
            {
                Query = new Query(new MatchAllQuery()),
                Aggs = new List<IAggs>
                {
                    new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
                    {
                        BackgroundFilter= new TermFilter("name", "details"),
                        InformationRetrievalRetrieval = new MutualInformation
                        {
                            BackgroundIsSuperset = false,
                            IncludeNegatives = false
                        },
                        ExecutionHint= ExecutionHint.global_ordinals
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetComplexValue<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
                Assert.Equal(7, aggResult.DocCount);
            }
        }

        [Fact]
        public void SearchAggGeohashGridBucketAggregationWithSubAggSignificantTermsBucketAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new GeohashGridBucketAggregation("termsDetails", "location")
                    {
                        Aggs = new List<IAggs>
                        {
                            new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
                        },
                        Precision = 5
                    }					
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var termsAggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("termsDetails");
                var significantAggResult = termsAggResult.Buckets[0].GetSubAggregationsFromJTokenName<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
                Assert.Equal(3, significantAggResult.Buckets[0].BgCount);
            }
        }

    }
}
