using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.Units;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class FilterBucketAggregationAndMissingBucketAggregationTests : SetupSearchAgg, IDisposable
    {
        public FilterBucketAggregationAndMissingBucketAggregationTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
		public void SearchAggFilterBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new FilterBucketAggregation("filterbucket", new MatchAllFilter())				
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FilterBucketAggregationsResult>("filterbucket");
				Assert.Equal(7, aggResult.DocCount);
			}
		}

		[Fact]
		public void SearchAggFilterBucketAggregationWithChildrenNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new FilterBucketAggregation("globalbucket", new GeoDistanceFilter("location", new GeoPoint(32,42), new DistanceUnitKilometer(10000) ))
					{
						Aggs = new List<IAggs>
						{
							new MaxMetricAggregation("maxAgg", "lift"),
							new MinMetricAggregation("minAgg", "lift"),
							new TopHitsMetricAggregation("topHits")
							{
								Size = 3
							}
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GlobalBucketAggregationsResult>("globalbucket");
				var max = aggResult.GetSingleMetricSubAggregationValue<double>("maxAgg");
				Assert.Equal(7, aggResult.DocCount);
				Assert.Equal(2.9, max);
			}
		}

		[Fact]
		public void SearchAggMissingBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new MissingBucketAggregation("missingbucket", "nonExistingFieldName")				
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<MissingBucketAggregationsResult>("missingbucket");
				Assert.Equal(7, aggResult.DocCount);
			}
		}

		[Fact]
		public void SearchAggMissingBucketAggregationWithTopHitsWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new MissingBucketAggregation("missingbucket", "nonExistingFieldName")
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
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<MissingBucketAggregationsResult>("missingbucket");
				Assert.Equal(7, aggResult.DocCount);
			}
		}

		[Fact]
		public void SearchAggFiltersBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new FiltersBucketAggregation("filtersbucket", new List<IFilter>
					{
						new MatchAllFilter(),
						new TermFilter("details", "document")
					})				
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersBucketAggregationsResult>("filtersbucket");
				Assert.Equal(7, aggResult.Buckets[0].DocCount);
			}
		}

		[Fact]
		public void SearchAggFiltersBucketAggregationWithTopHitsWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new FiltersBucketAggregation("filtersbucket", 
						new List<IFilter>
						{
							new MatchAllFilter(),
							new TermFilter("details", "document")
						})
					{
						Aggs = new List<IAggs>
						{
							new TopHitsMetricAggregation("data")
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersBucketAggregationsResult>("filtersbucket");
				var results = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("data");
				Assert.Equal(7, aggResult.Buckets[0].DocCount);
				Assert.Equal(7, results.Hits.Total);
			}
		}

		[Fact]
		public void SearchAggFiltersNamedBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new FiltersNamedBucketAggregation("filtersbucket", new List<NamedFilter>
					{
						new NamedFilter("all", new MatchAllFilter()),
						new NamedFilter("detailsTerm", new TermFilter("details", "document"))
					})				
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersNamedBucketAggregationsResult>("filtersbucket");
				Assert.Equal(7, aggResult.Buckets.GetSubAggregationsFromJTokenName<BaseBucket>("all").DocCount);
			}
		}

		[Fact]
		public void SearchAggFiltersNamedBucketAggregationWithTopHitsWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new FiltersNamedBucketAggregation("filtersbucket", new List<NamedFilter>
					{
						new NamedFilter("all", new MatchAllFilter()),
						new NamedFilter("detailsTerm", new TermFilter("details", "document"))
					})		
					{
						Aggs = new List<IAggs>
						{
							new TopHitsMetricAggregation("data")
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersNamedBucketAggregationsResult>("filtersbucket");
				Assert.Equal(7, aggResult.Buckets.GetSubAggregationsFromJTokenName<BaseBucket>("all").DocCount);

				var results = aggResult.Buckets.GetSubAggregationsFromJTokenName<BaseBucket>("all").GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("data");
				Assert.Equal(7, results.Hits.Total);
			}
		}
    }
}
