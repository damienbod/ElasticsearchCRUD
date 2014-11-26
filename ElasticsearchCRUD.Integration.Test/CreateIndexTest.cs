using System.Threading;
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
				if (context.IndexExists<MappingTestsParent>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult1 = context.DeleteIndexAsync<MappingTestsParent>();
					entityResult1.Wait();
				}
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayList()
		{
			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.CreateIndex<MappingTestsParent>();

				Thread.Sleep(1500);
				var result = context.IndexExists<MappingTestsParent>();
				Assert.IsTrue(result);
			}
		}
	}
}
