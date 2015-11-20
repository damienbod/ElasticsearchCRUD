using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;

using Xunit;

namespace ElasticsearchCRUD.Integration.Test.OneToN 
{
	public class NestedArraysTestsWithFiltersAndQueriesAndAggregations : IDisposable
	{
		private List<SkillChild> _entitiesForSkillChild;
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[Fact]
		public void TestDefaultContextParentWithACollectionOfThreeChildObjectsOfNestedType()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetDocument<NestedCollectionTest>(testSkillParentObject.Id);
				Assert.AreEqual(roundTripResult.DescriptionSkillParent, testSkillParentObject.DescriptionSkillParent);
				Assert.AreEqual(roundTripResult.SkillChildren.First().DescriptionSkillChild, testSkillParentObject.SkillChildren.First().DescriptionSkillChild);
				Assert.AreEqual(roundTripResult.SkillChildren.ToList()[1].DescriptionSkillChild, testSkillParentObject.SkillChildren.ToList()[1].DescriptionSkillChild);
				Assert.AreEqual(roundTripResult.SkillChildren.ToList()[1].DescriptionSkillChild, testSkillParentObject.SkillChildren.ToList()[1].DescriptionSkillChild);
			}
		}

		[Fact]
		public void SearchFilterNestedFilter()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);
			var search = new Search
			{
				Filter =
					new Filter(
					new NestedFilter(
					new MatchAllFilter(), "skillchildren"))
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search);
				Assert.AreEqual(8, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchFilterNestedFilterWithInnerHitsDefault()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);
			var search = new Search
			{
				Filter =
					new Filter(
					new NestedFilter(
					new MatchAllFilter(), "skillchildren")
						{
							InnerHits= new InnerHits()
						})
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search);
				Assert.AreEqual(8, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchFilterNestedQuery()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);
			var search = new Search
			{
				Query =
					new Query(
					new NestedQuery(
					new MatchAllQuery(), "skillchildren"))
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search);
				Assert.AreEqual(8, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchFilterNestedQueryInnerHits()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);
			var search = new Search
			{
				Query =
					new Query(
					new NestedQuery(
					new MatchAllQuery(), "skillchildren")
					{
						InnerHits = new InnerHits()
					})
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search);
				Assert.Equal(8, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchAggNestedBucketAggregationWithNoHits()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);

			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new NestedBucketAggregation("nestedagg", "skillchildren")				
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<NestedBucketAggregationsResult>("nestedagg");
				Assert.AreEqual(3, aggResult.DocCount);
			}
		}

		[Fact]
		public void SearchAggNestedBucketAggregationWithSubAggMaxMetricWithNoHits()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);

			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new NestedBucketAggregation("nestedagg", "skillchildren")
					{
						Aggs = new List<IAggs>
						{
							new MaxMetricAggregation("test", "skillchildren.updatedskillchild")
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<NestedBucketAggregationsResult>("nestedagg");
				var max = aggResult.GetSingleMetricSubAggregationValue<long>("test");
				Assert.AreEqual(3, aggResult.DocCount);
				Assert.Greater(max, 1423210851080);
			}
		}

		[Fact]
		public void SearchAggNestedBucketAggregationWithSubReverseNestedWithNoHits()
		{
			var testSkillParentObject = new NestedCollectionTest
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<NestedCollectionTest>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(testSkillParentObject, testSkillParentObject.Id);
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}

			Thread.Sleep(1200);

			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new NestedBucketAggregation("nestedagg", "skillchildren")
					{
						Aggs = new List<IAggs>
						{
							new MaxMetricAggregation("test", "skillchildren.updatedskillchild"),
							new ReverseNestedBucketAggregation("goingUp")
							{
								Aggs = new List<IAggs>
								{
									new TermsBucketAggregation("termParent", "descriptionskillparent")
								}
							}
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<NestedCollectionTest>());
				var items = context.Search<NestedCollectionTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<NestedBucketAggregationsResult>("nestedagg");
				var max = aggResult.GetSingleMetricSubAggregationValue<long>("test");
				var nesteTestResult = aggResult.GetSubAggregationsFromJTokenName<ReverseNestedBucketAggregationsResult>("goingUp");
				var termParentAgg = nesteTestResult.GetSubAggregationsFromJTokenName<TermsBucketAggregationsResult>("termParent");

				Assert.AreEqual(3, aggResult.DocCount);
				Assert.Greater(max, 1423210851080);
				Assert.AreEqual(1, nesteTestResult.DocCount);
				Assert.AreEqual("a", termParentAgg.Buckets[0].Key);
			}
		}

		[SetUp]
		public void SetUp()
		{
			_entitiesForSkillChild = new List<SkillChild>();

			for (int i = 0; i < 3; i++)
			{
				var entityTwo = new SkillChild
				{
					CreatedSkillChild = DateTime.UtcNow,
					UpdatedSkillChild = DateTime.UtcNow,
					DescriptionSkillChild = "A test SkillChild description",
					Id = i,
					NameSkillChild = "cool"
				};

				_entitiesForSkillChild.Add(entityTwo);
			}
		}

		[TearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult14 = context.DeleteIndexAsync<NestedCollectionTest>(); entityResult14.Wait();
			}
		}

	    public void Dispose()
	    {
	        throw new NotImplementedException();
	    }
	}

	public class NestedCollectionTest
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }
		public string DescriptionSkillParent { get; set; }
		public DateTimeOffset CreatedSkillParent { get; set; }
		public DateTimeOffset UpdatedSkillParent { get; set; }

		[ElasticsearchNested(IncludeInParent = true)]
		public virtual Collection<SkillChild> SkillChildren { get; set; }
	}
}
