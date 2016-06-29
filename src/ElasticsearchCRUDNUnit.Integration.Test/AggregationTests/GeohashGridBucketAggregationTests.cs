using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class GeohashGridBucketAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggGeohashGridBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new GeohashGridBucketAggregation("testGeohashGridBucketAggregation", "location")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}

		[Test]
		public void SearchAggGeohashGridBucketAggregationWithTopHitsSubWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
				var topHits = aggResult.Buckets[0].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("tophits");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
				Assert.AreEqual(3, topHits.Hits.Total);
			}
		}

		[Test]
		public void SearchAggGeohashGridBucketAggregationPrecisionWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}

		[Test]
		public void SearchAggTermsBucketAggregationScriptWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeohashGridBucketAggregationsResult>("testGeohashGridBucketAggregation");
				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);
			}
		}
	}
}
