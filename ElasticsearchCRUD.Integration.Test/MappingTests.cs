
using System.Runtime.CompilerServices;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class MappingTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[Test]
		public void CreateNewIndexAndMappingForNestedChild()
		{
			var mappingTestsParent = new MappingTestsParent
			{
				Calls = 3,
				MappingTestsParentId = 2,
				MappingTestsItem = new MappingTestsChild
				{
					Description = "Hello nested",
					MappingTestsChildId = 5
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{				
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappingsForChildDocuments();	
			}
		}
	}

	public class MappingTestsParent
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store=true)]
		public int Calls { get; set; }

		public MappingTestsChild MappingTestsItem { get; set; }
	}

	public class MappingTestsChild
	{
		public int MappingTestsChildId { get; set; }

		[ElasticsearchString(Boost=1.7)]
		public string Description { get; set; }

	}
}
