using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class CreateIndexTest
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				if (context.IndexExists<MappingTestsParent>())
				{					
					var entityResult1 = context.DeleteIndexAsync<MappingTestsParent>();
					entityResult1.Wait();
				}

				if (context.IndexExists<AliasCreateIndexOne>())
				{
					var entityResult2 = context.DeleteIndexAsync<AliasCreateIndexOne>();
					entityResult2.Wait();
				}

				if (context.IndexExists<AliasCreateIndexTwo>())
				{
					var entityResult3 = context.DeleteIndexAsync<AliasCreateIndexTwo>();
					entityResult3.Wait();
				}
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayList()
		{
			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTestsParent>();

				Thread.Sleep(1500);
				var result = context.IndexExists<MappingTestsParent>();
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayListAndAlias()
		{
			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<AliasCreateIndexOne>(new IndexDefinition { IndexAliases = new IndexAliases { Aliases = new List<IndexAlias> { new IndexAlias("testnew"), new IndexAlias("testnewroute") { Routing = "ddd" } } } });

				Thread.Sleep(1500);
				var result = context.IndexExists<AliasCreateIndexOne>();
				Assert.IsTrue(result);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayListAndAlias2()
		{
			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate("aliascreateindextwos", new IndexSettings(), new IndexAliases { Aliases = new List<IndexAlias> { new IndexAlias("testhhhnew"), new IndexAlias("testnewroute") { Routing = "dhd" } } });

				Thread.Sleep(1500);
				var result = context.IndexExists<AliasCreateIndexTwo>();
				Assert.IsTrue(result);
			}
		}
	}


	public class AliasCreateIndexOne
	{
		public int Id { get; set; }

		public string data { get; set; }
	}

	public class AliasCreateIndexTwo
	{
		public int Id { get; set; }

		public string data { get; set; }
	}
}
