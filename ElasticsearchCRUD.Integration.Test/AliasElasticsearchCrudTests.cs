using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextAlias.AliasModel;
using ElasticsearchCRUD.Utils;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class AliasElasticsearchCrudTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
		private const string ConnectionString = "http://localhost.fiddler:9200";

		private void WaitForDataOrFail()
		{
			if (!_resetEvent.WaitOne(5000))
			{
				Assert.Fail("No data received within specified time");
			}
		}
		[TearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<IndexAliasDtoTest>();
				entityResult.Wait();

				var secondDelete = context.DeleteIndexAsync<IndexAliasDtoTestTwo>();
				secondDelete.Wait();		
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException))]
		public void CreateAliasForNoExistingIndex()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex("test", "doesnotexistindex");
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchCrudJsonWriter: index is not allowed in Elasticsearch: doeGGGtindex")]
		public void CreateAliasForIndexBadIndex()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex("test", "doeGGGtindex");
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchCrudJsonWriter: index is not allowed in Elasticsearch: tesHHHt")]
		public void CreateAliasForIndexBadAlias()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex("tesHHHt", "doendex");
			}
		}

		[Test]
		public void CreateAliasForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest {Id = 1, Description = "Test index for aliases"};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest,  indexAliasDtoTest.Id);
				context.SaveChanges();

				var result = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(result);
			}
		}


		[Test]
		public void CreateAliasForIndex2()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };

			var aliasParameters = new AliasParameters
			{
				Actions = new List<AliasBaseParameters>
				{
					new AliasAddParameters("test2", "indexaliasdtotests"),
					new AliasAddParameters("test3", "indexaliasdtotests")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var result = context.Alias(aliasParameters.ToString());
				Assert.IsTrue(result);

				Assert.IsTrue(context.AliasExists("test2"));
				Assert.IsTrue(context.AliasExists("test3"));
				
			}
		}

		[Test]
		[ExpectedException(ExpectedException=typeof(ElasticsearchCrudException),ExpectedMessage="ElasticSearchContextGet: HttpStatusCode.NotFound")]
		public void CreateAliasForIndex3()
		{
			var indexAliasDtoTest3 = new IndexAliasDtoTest { Id = 3, Description = "no" };
			var indexAliasDtoTest4 = new IndexAliasDtoTest { Id = 4, Description = "boo" };
			var indexAliasDtoTest5 = new IndexAliasDtoTest { Id = 5, Description = "boo" };

			var aliasParameters = new AliasParameters
			{
				Actions = new List<AliasBaseParameters>
				{
					new AliasAddParameters("test4", "indexaliasdtotests")
					{
						Routing="newroute",
						Filter= "{ \"term\" : { \"description\" : \"boo\" } }"
					}
				}
			};

			const bool userDefinedRouting = true;
			var elasticsearchSerializerConfiguration = new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver,
				true, false, userDefinedRouting);

			using (var context = new ElasticsearchContext(ConnectionString, elasticsearchSerializerConfiguration))
			{
				context.AddUpdateDocument(indexAliasDtoTest3, indexAliasDtoTest3.Id, new RoutingDefinition { RoutingId = "newroute" });
				context.AddUpdateDocument(indexAliasDtoTest4, indexAliasDtoTest4.Id, new RoutingDefinition { RoutingId = "newroute" });
				context.AddUpdateDocument(indexAliasDtoTest5, indexAliasDtoTest5.Id, new RoutingDefinition { RoutingId = "newroute" });
				context.SaveChanges();

				var result = context.Alias(aliasParameters.ToString());
				Assert.IsTrue(result);

				Assert.IsTrue(context.AliasExists("test4"));

				// using the index
				var doc3 = context.GetDocument<IndexAliasDtoTest>(3, new RoutingDefinition {RoutingId = "newroute"});
				Assert.IsTrue(doc3.Id == 3);
				var doc4 = context.GetDocument<IndexAliasDtoTest>(4, new RoutingDefinition { RoutingId = "newroute" });
				Assert.IsTrue(doc4.Id == 4);

			}

			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();

			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(
				typeof(IndexAliasDtoTest), 
				MappingUtils.GetElasticsearchMapping<IndexAliasDtoTest>("test4", "indexaliasdtotest")
			);

			using (var context = new ElasticsearchContext(ConnectionString, elasticsearchMappingResolver))
			{
				// using the alias
				var xx = context.GetDocument<IndexAliasDtoTest>(4);
				Assert.IsTrue(xx.Id == 4);

				// should not be found due to filter
				var notfound = context.GetDocument<IndexAliasDtoTest>(3);
			}
		}

		[Test]
		public void RemoveAliasForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var resultCreate = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultCreate);

				var resultRemove = context.AliasRemoveForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultRemove);
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException))]
		public void RemoveAliasthatDoesNotExistForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var result = context.AliasRemoveForIndex("tefdfdfdsfst", "indexaliasdtotests");
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void ReplaceIndexForAlias()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };
			var indexAliasDtoTestTwo = new IndexAliasDtoTestTwo { Id = 1, Description = "Test Doc Type Two index for aliases" };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.AddUpdateDocument(indexAliasDtoTestTwo, indexAliasDtoTestTwo.Id);
				context.SaveChanges();

				var resultCreate = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultCreate);

				var result = context.AliasReplaceIndex("test", "indexaliasdtotests", "indexaliasdtotesttwos");
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void ReindexTest()
		{
			var indexAliasDtoTestV1 = new IndexAliasDtoTestThree { Id = 1, Description = "V1" };
			var indexAliasDtoTestV2 = new IndexAliasDtoTestThree { Id = 2, Description = "V2" };

			IElasticsearchMappingResolver elasticsearchMappingResolverDirectIndex = new ElasticsearchMappingResolver();
			IElasticsearchMappingResolver elasticsearchMappingResolverDirectIndexV1 = new ElasticsearchMappingResolver();
			var mappingV1 = new IndexAliasDtoTestThreeMappingV1();

			IElasticsearchMappingResolver elasticsearchMappingResolverDirectIndexV2 = new ElasticsearchMappingResolver();
			var mappingV2 = new IndexAliasDtoTestThreeMappingV2();

			elasticsearchMappingResolverDirectIndexV1.AddElasticSearchMappingForEntityType(typeof(IndexAliasDtoTestThree), mappingV1);
			elasticsearchMappingResolverDirectIndexV2.AddElasticSearchMappingForEntityType(typeof(IndexAliasDtoTestThree), mappingV2);

			// Step 1 create index V1 and add alias
			using (var context = new ElasticsearchContext(ConnectionString, elasticsearchMappingResolverDirectIndexV1))
			{
				// create the index
				context.AddUpdateDocument(indexAliasDtoTestV1, indexAliasDtoTestV1.Id);
				context.SaveChanges();

				var resultCreate = context.AliasCreateForIndex("indexaliasdtotestthrees", "indexaliasdtotestthree_v1");
				Assert.IsTrue(resultCreate);
			}

			// Step 2 create index V2 and replace alias
			using (var context = new ElasticsearchContext(ConnectionString, elasticsearchMappingResolverDirectIndexV2))
			{
				// create the index
				context.AddUpdateDocument(indexAliasDtoTestV2, indexAliasDtoTestV2.Id);
				context.SaveChanges();

				var result = context.AliasReplaceIndex("indexaliasdtotestthrees", "indexaliasdtotestthree_v1", "indexaliasdtotestthree_v2");
				Assert.IsTrue(result);
			}


			Task.Run(() =>
			{
				using (var context = new ElasticsearchContext(ConnectionString, elasticsearchMappingResolverDirectIndex))
				{
					while (true)
					{
						Thread.Sleep(1000);
						var itemOk = context.SearchById<IndexAliasDtoTestThree>(2);
						if (itemOk != null)
						{
							_resetEvent.Set();
						}
						
					}
				}
// ReSharper disable once FunctionNeverReturns
			});


			WaitForDataOrFail();

			// delete index v1
			using (var context = new ElasticsearchContext(ConnectionString, elasticsearchMappingResolverDirectIndexV1))
			{
				context.AllowDeleteForIndex = true;
				var thirdDelete = context.DeleteIndexAsync<IndexAliasDtoTestThree>();
				thirdDelete.Wait();
			}

			// delete index v2
			using (var context = new ElasticsearchContext(ConnectionString, elasticsearchMappingResolverDirectIndexV2))
			{
				context.AllowDeleteForIndex = true;
				var thirdDelete = context.DeleteIndexAsync<IndexAliasDtoTestThree>();
				thirdDelete.Wait();
			}

		}

		[Test]
		public void RenameAliasForIndex()
		{
			var indexAliasDtoTest = new IndexAliasDtoTest { Id = 1, Description = "Test index for aliases" };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AddUpdateDocument(indexAliasDtoTest, indexAliasDtoTest.Id);
				context.SaveChanges();

				var resultCreate = context.AliasCreateForIndex("test", "indexaliasdtotests");
				Assert.IsTrue(resultCreate);

				var result = context.Alias(BuildRenameAliasJsonContent("test", "testNew", "indexaliasdtotests"));
				Assert.IsTrue(result);
			}
		}

		private string BuildRenameAliasJsonContent(string aliasOld, string aliasNew, string index)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			sb.AppendLine("\"actions\" : [");
			sb.AppendLine("{ \"remove\" : { \"index\" : \"" + index + "\", \"alias\" : \"" + aliasOld + "\" } },");
			sb.AppendLine("{ \"add\" : { \"index\" : \"" + index + "\", \"alias\" : \"" + aliasNew + "\" } }");
			sb.AppendLine("]");
			sb.AppendLine("}");

			return sb.ToString();
		}
	}

	public class IndexAliasDtoTest
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}

	public class IndexAliasDtoTestTwo
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}

	public class IndexAliasDtoTestThree
	{
		public long Id { get; set; }
		public string Description { get; set; }
	}

	public class IndexAliasDtoTestThreeMappingV1 : ElasticsearchMapping
	{
		public override string GetIndexForType(Type type)
		{
			return "indexaliasdtotestthree_v1";
		}
	}

	public class IndexAliasDtoTestThreeMappingV2 : ElasticsearchMapping
	{
		public override string GetIndexForType(Type type)
		{
			return "indexaliasdtotestthree_v2";
		}
	}
}
