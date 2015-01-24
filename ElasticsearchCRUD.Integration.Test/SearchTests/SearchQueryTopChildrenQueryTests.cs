using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryTopChildrenQueryTests
	{
		protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
		protected const string ConnectionString = "http://localhost:9200";

		[Test]
		public void SearchQueryMoreLikeThisQueryMatchAll()
		{
			var search = new Search
			{
				Query = new Query(new TopChildrenQuery("testobjchildsep",
					new MatchAllQuery())
					{
						Factor=5,
						IncrementalFactor= 2,
						Score= Score.sum
					}	
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());
				var items = context.Search<TestObjParentSep>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryMoreLikeThisQueryMatchOne()
		{
			var search = new Search
			{
				Query = new Query(new TopChildrenQuery("testobjchildsep",
					new MatchQuery("details", "three"))
				{
					Factor = 5,
					IncrementalFactor = 2,
					Score = Score.sum
				}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());
				var items = context.Search<TestObjParentSep>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.Total);
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
				Info = "yes this is great two",
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
				Thread.Sleep(1000);
				context.AddUpdateDocument(doc1, doc1.Id);
				context.AddUpdateDocument(doc2, doc2.Id);
				context.AddUpdateDocument(doc3, doc3.Id);
				context.SaveChanges();
				Thread.Sleep(1000);
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
