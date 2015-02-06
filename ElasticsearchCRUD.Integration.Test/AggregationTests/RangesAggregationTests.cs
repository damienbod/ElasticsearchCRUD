using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class RangessAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggRangesBucketAggregationWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("2.0", aggResult.Buckets[2].FromAsString);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationKeyedWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
				var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("2.0-*");

				Assert.AreEqual("2.0", test.FromAsString);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationWithRangeKeysWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("three", aggResult.Buckets[2].Key);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationKeyedWithRangeKeysWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
				var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("three");

				Assert.AreEqual("2.0", test.FromAsString);
			}
		}

		[Test]
		public void SearchAggRangesBucketAggregationWithRangeKeysWithMaxMetricSubAggWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				var max = aggResult.Buckets[2].GetSingleMetricSubAggregationValue<double>("maxi");
				Assert.AreEqual(6, aggResult.Buckets[2].DocCount);
				Assert.AreEqual("three", aggResult.Buckets[2].Key);
				Assert.AreEqual(2.9, max);
			}
		}

		[Test]
		public void SearchAggDateRangesBucketAggregationWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesBucketAggregationsResult>("testRangesBucketAggregation");
				Assert.AreEqual(7, aggResult.Buckets[2].DocCount);
			}
		}

		[Test]
		public void SearchAggDateRangesBucketAggregationKeyedWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<RangesNamedBucketAggregationsResult>("testRangesBucketAggregation");
				var test = aggResult.Buckets.GetSubAggregationsFromJTokenName<RangeBucket>("keyName");

				Assert.AreEqual(7, test.DocCount);			
			}
		}

		[Test]
		public void SearchAggGeoDistanceBucketAggregationWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoDistanceBucketAggregationsResult>("testGeoDistanceBucketAggregation");
				Assert.AreEqual(7, aggResult.Buckets[2].DocCount);
				Assert.AreEqual(500, aggResult.Buckets[2].From);
			}
		}

		[Test]
		public void SearchAggGeoDistanceBucketAggregationWithDistanceTypeWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoDistanceBucketAggregationsResult>("testGeoDistanceBucketAggregation");
				Assert.AreEqual(7, aggResult.Buckets[2].DocCount);
				Assert.AreEqual(500, aggResult.Buckets[2].From);
			}
		}

		[Test]
		public void SearchAggGeoDistanceBucketAggregationWithDistanceTypeWithTopHitsWithNoHits()
		{
			var search = new Search
			{
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
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoDistanceBucketAggregationsResult>("testGeoDistanceBucketAggregation");
				var hits = aggResult.Buckets[2].GetSubAggregationsFromJTokenName<TopHitsMetricAggregationsResult<SearchAggTest>>("tops");
				Assert.AreEqual(7, aggResult.Buckets[2].DocCount);
				Assert.AreEqual(500, aggResult.Buckets[2].From);
				Assert.AreEqual(7, hits.Hits.Total);
			}
		}
	}
}
