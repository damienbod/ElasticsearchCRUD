using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryFilterTestsMoreLikeThis
	{
		protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
		protected const string ConnectionString = "http://localhost:9200";

		[Test]
		public void SearchQueryMoreLikeThisQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new MoreLikeThisQuery
					{
						Fields = new List<string> {"moreinfo"},
						LikeText = "yes one",
						MinTermFreq = 1,
						Boost = 50,
						MinDocFreq = 1,
                        MinimumShouldMatch = "20%"

					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<MoreLikeThisTestDto>());
				var items = context.Search<MoreLikeThisTestDto>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			var doc1 = new MoreLikeThisTestDto
			{
				Id = 1,
				Info = "yes this is great",
				MoreInfo = "yes, I am going to test this query"
			};

			var doc2 = new MoreLikeThisTestDto
			{
				Id = 2,
				Info = "yes this is great two",
				MoreInfo = "yes, I am going to test this query"
			};

			var doc3 = new MoreLikeThisTestDto
			{
				Id = 3,
				Info = "yes this is great three",
				MoreInfo = "no, I am going to test this query"
			};


			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchMappingResolver()))
			{
				context.IndexCreate<MoreLikeThisTestDto>();
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
				var entityResult = context.DeleteIndexAsync<MoreLikeThisTestDto>(); entityResult.Wait();
			}
		}
	}

	public class MoreLikeThisTestDto
	{
		[ElasticsearchId]
		public long Id { get; set; }

		public string Info { get; set; }

		public string MoreInfo { get; set; }
	}
}
