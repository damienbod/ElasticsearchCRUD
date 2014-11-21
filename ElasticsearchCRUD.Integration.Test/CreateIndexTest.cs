using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class CreateIndexTest
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayList()
		{
			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.CreateIndex<MappingTestsParent>(new RoutingDefinition());

				Thread.Sleep(1500);
				var result = context.IndexExists<MappingTestsParent>();
				Assert.IsTrue(result);
			}
		}
	}
}
