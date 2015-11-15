using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{

	[TestFixture]
	public class SearchQueryFilterTestsOneToNParentChild
	{
		protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
		protected const string ConnectionString = "http://localhost.fiddler:9200";

		[Test]
		public void SearchFilterHasChildFilter()
		{
			var search = new Search { Filter = 
				new Filter(
					new HasChildFilter("testobjchildsep", new MatchAllFilter())
					{
						MaxChildren= 10,
						MinChildren=2
					}
				) 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				var items = context.Search<TestObjParentSep>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilterHasChildFilterInnerHits()
		{
			var search = new Search
			{
				Filter =
					new Filter(
					new HasChildFilter("testobjchildsep", new MatchAllFilter())
					{
						MaxChildren = 10,
						MinChildren = 2,
						InnerHits = new InnerHits()
					}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				var items = context.Search<TestObjParentSep>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilterHasParentFilter()
		{
			var search = new Search
			{
				Filter =
					new Filter(
					new HasParentFilter("testobjparentsep", new MatchAllFilter())
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());
				var items = context.Search<TestObjChildSep>(search);
				Assert.AreEqual(4, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchFilterHasParentFilterInnerHits()
		{
			var search = new Search
			{
				Filter =
					new Filter(
					new HasParentFilter("testobjparentsep", new MatchAllFilter())
						{
							InnerHits= new InnerHits()
						}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());
				var items = context.Search<TestObjChildSep>(search);
				Assert.AreEqual(4, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchFilterHasChildQuery()
		{
			var search = new Search
			{
				Query =
					new Query(
						new HasChildQuery("testobjchildsep", new MatchAllQuery())
						{
							MaxChildren = 10,
							MinChildren = 2,
							ScoreMode=ScoreMode.none 
						}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				var items = context.Search<TestObjParentSep>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilterHasChildQueryInnerHits()
		{
			var search = new Search
			{
				Query =
					new Query(
						new HasChildQuery("testobjchildsep", new MatchAllQuery())
						{
							MaxChildren = 10,
							MinChildren = 2,
							ScoreMode = ScoreMode.none,
							InnerHits= new InnerHits()
						}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjParentSep>());
				var items = context.Search<TestObjParentSep>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilterHasParentQuery()
		{
			var search = new Search
			{
				Query =
					new Query(
					new HasParentQuery("testobjparentsep", new MatchAllQuery())
					{
						ScoreMode= ScoreModeHasParentQuery.none
					}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());
				var items = context.Search<TestObjChildSep>(search);
				Assert.AreEqual(4, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchFilterHasParentQueryInnerHits()
		{
			var search = new Search
			{
				Query =
					new Query(
					new HasParentQuery("testobjparentsep", new MatchAllQuery())
					{
						ScoreMode = ScoreModeHasParentQuery.none,
						InnerHits = new InnerHits()
					}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<TestObjChildSep>());
				var items = context.Search<TestObjChildSep>(search);
				Assert.AreEqual(4, items.PayloadResult.Hits.Total);
			}
		}

		[TestFixtureSetUp]
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

	public class TestObjParentSep
	{
		[ElasticsearchId]
		public long Id { get; set; }

		public string Info { get; set; }

		public List<TestObjChildSep> ChildObjects { get; set; }
	}

	public class TestObjChildSep
	{
		[ElasticsearchId]
		public long Id { get; set; }

		public string Details { get; set; }
	}
}
