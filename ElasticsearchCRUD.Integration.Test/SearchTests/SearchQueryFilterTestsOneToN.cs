using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	/// <summary>
	/// TODO support nested mapping 
	/// </summary>
	[TestFixture]
	public class SearchQueryFilterTestsOneToN
	{
		protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
		protected const string ConnectionString = "http://localhost.fiddler:9200";

		[Test]
		public void SearchFilterNestedFilter()
		{
			var search = new Search { Filter = 
				new Filter(
					new NestedFilter(
						new MatchAllFilter(), "childobjects")) 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			var doc1 = new TestObjParent
			{
				Id = 1,
				Info = "yes this is great",
				ChildObjects = new List<TestObjChild>
				{
					new TestObjChild
					{
						Id=1,
						Details="my child"
					}
				}
			};

			var doc2 = new TestObjParent
			{
				Id = 2,
				Info = "yes this is great two",
				ChildObjects = new List<TestObjChild>
				{
					new TestObjChild
					{
						Id = 1,
						Details = "my child"
					}
				}
			};

			var doc3 = new TestObjParent
			{
				Id = 3,
				Info = "yes this is great three",
				ChildObjects = new List<TestObjChild>
				{
					new TestObjChild
					{
						Id = 3,
						Details = "my child three"
					},
					new TestObjChild
					{
						Id = 4,
						Details = "my child four"
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				//context.IndexCreate<TestObjParent>();
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
				//var entityResult = context.DeleteIndexAsync<TestObjParent>(); entityResult.Wait();
			}
		}
	}

	public class TestObjParent
	{
		[ElasticsearchId]
		public long Id { get; set; }

		public string Info { get; set; }

		public List<TestObjChild> ChildObjects { get; set; }
	}

	public class TestObjChild
	{
		[ElasticsearchId]
		public long Id { get; set; }

		public string Details { get; set; }
	}
}
