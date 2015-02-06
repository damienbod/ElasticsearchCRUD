using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.Units;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class FilterBucketAggregationAndMissingBucketAggregationTests : SetupSearchAgg
	{
		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FilterBucketAggregationsResult>("filterbucket");
				Assert.AreEqual(7, aggResult.DocCount);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GlobalBucketAggregationsResult>("globalbucket");
				var max = aggResult.GetSingleMetricSubAggregationValue<double>("maxAgg");
				Assert.AreEqual(7, aggResult.DocCount);
				Assert.AreEqual(2.9, max);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<MissingBucketAggregationsResult>("missingbucket");
				Assert.AreEqual(7, aggResult.DocCount);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<MissingBucketAggregationsResult>("missingbucket");
				Assert.AreEqual(7, aggResult.DocCount);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersBucketAggregationsResult>("filtersbucket");
				Assert.AreEqual(7, aggResult.Buckets[0].DocCount);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersBucketAggregationsResult>("filtersbucket");
				var results = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("data");
				Assert.AreEqual(7, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(7, results.Hits.Total);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersNamedBucketAggregationsResult>("filtersbucket");
				Assert.AreEqual(7, aggResult.Buckets.GetSubAggregationsFromJTokenName<BaseBucket>("all").DocCount);
			}
		}

		[Test]
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<FiltersNamedBucketAggregationsResult>("filtersbucket");
				Assert.AreEqual(7, aggResult.Buckets.GetSubAggregationsFromJTokenName<BaseBucket>("all").DocCount);

				var results = aggResult.Buckets.GetSubAggregationsFromJTokenName<BaseBucket>("all").GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("data");
				Assert.AreEqual(7, results.Hits.Total);
			}
		}

	}
}
