using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class MappingTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult1 = context.DeleteIndexAsync<MappingTestsParent>();
				entityResult1.Wait();

				context.AllowDeleteForIndex = true;
				var entityResult2 = context.DeleteIndexAsync<MappingTestsParentWithList>();
				entityResult2.Wait();

				context.AllowDeleteForIndex = true;
				var entityResult3 = context.DeleteIndexAsync<MappingTestsParentWithArray>();
				entityResult3.Wait();
			}
		}

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

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParent>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingForNestedListOfChild()
		{
			var mappingTestsParent = new MappingTestsParentWithList
			{
				Calls = 3,
				MappingTestsParentId = 3,
				MappingTestsItemList = new List<MappingTestsChild>
				{
					new MappingTestsChild
					{
						Description = "Hello nested",
						MappingTestsChildId = 6
					},
					new MappingTestsChild
					{
						Description = "Hello nested item in list",
						MappingTestsChildId = 7
					}
				}

			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentWithList>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
			}
		}


		[Test]
		public void CreateNewIndexAndMappingForNestedArrayOfChild()
		{
			var mappingTestsParent = new MappingTestsParentWithArray
			{
				Calls = 3,
				MappingTestsParentId = 3,
				MappingTestsItemArray = new[]
				{
					new MappingTestsChild
					{
						Description = "Hello nested",
						MappingTestsChildId = 6
					},
					new MappingTestsChild
					{
						Description = "Hello nested item in list",
						MappingTestsChildId = 7
					}
				}

			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentWithArray>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
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

	public class MappingTestsParentWithList
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		public List<MappingTestsChild> MappingTestsItemList { get; set; }
	}

	public class MappingTestsParentWithArray
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		public MappingTestsChild[] MappingTestsItemArray { get; set; }
	}
}