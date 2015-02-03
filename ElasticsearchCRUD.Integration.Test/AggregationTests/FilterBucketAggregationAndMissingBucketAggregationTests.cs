using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
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

	}
}
