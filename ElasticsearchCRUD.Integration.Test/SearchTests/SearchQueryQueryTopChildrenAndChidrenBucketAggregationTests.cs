using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Utils;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryQueryTopChildrenAndChidrenBucketAggregationTests
	{
		protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
		protected const string ConnectionString = "http://localhost.fiddler:9200";

	
		[Test]
		public void SearchAggTermsBucketAggregationnWithChildrenSubAggNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("test_min", "info")
					{
						Aggs = new List<IAggs>
						{
							new ChidrenBucketAggregation("childrenAgg", "testobjchildsep")
							{
								Aggs = new List<IAggs>
								{
									new TermsBucketAggregation("infoAgg", "info")
								}
							}
						} 
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());

				var items = context.Search<TestObjParentSep>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("test_min");

				Assert.AreEqual(3, aggResult.Buckets[0].DocCount);

			}
		}

		[TestFixtureSetUpAttribute]
		public void Setup()
		{
			var doc1 = new TestObjParentSep
			{
				Id = 1,
				Info = "yes this is great",
				ChildObjects = new List<TestObjChildSep>
				{
					new TestObjChildSep
					{
						Id=1,
						Details="my child"
					}
				}
			};

			var doc2 = new TestObjParentSep
			{
				Id = 2,
				Info = "yes this is great two child",
				ChildObjects = new List<TestObjChildSep>
				{
					new TestObjChildSep
					{
						Id = 1,
						Details = "my child"
					}
				}
			};

			var doc3 = new TestObjParentSep
			{
				Id = 3,
				Info = "yes this is great three",
				ChildObjects = new List<TestObjChildSep>
				{
					new TestObjChildSep
					{
						Id = 3,
						Details = "my child three"
					},
					new TestObjChildSep
					{
						Id = 4,
						Details = "my child four"
					}
				}
			};

			ElasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(TestObjChildSep), 
				MappingUtils.GetElasticsearchMapping<TestObjChildSep>("testobjparentseps", "testobjchildsep"));

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(ElasticsearchMappingResolver,true,true)))
			{
				context.IndexCreate<TestObjParentSep>();
				Thread.Sleep(1200);
				context.AddUpdateDocument(doc1, doc1.Id);
				context.AddUpdateDocument(doc2, doc2.Id);
				context.AddUpdateDocument(doc3, doc3.Id);
				context.SaveChanges();
				Thread.Sleep(1200);
			}
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<TestObjParentSep>(); entityResult.Wait();
			}
		}
	}
}
