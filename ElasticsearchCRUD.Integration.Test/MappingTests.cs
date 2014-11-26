using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
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
				if (context.IndexExists<MappingTestsParent>())
				{
					context.AllowDeleteForIndex = true;
					context.DeleteIndex<MappingTestsParent>();
				}
				if (context.IndexExists<MappingTestsParentWithList>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult2 = context.DeleteIndexAsync<MappingTestsParentWithList>();
					entityResult2.Wait();
				}
				if (context.IndexExists<MappingTestsParentWithArray>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult3 = context.DeleteIndexAsync<MappingTestsParentWithArray>();
					entityResult3.Wait();
				}
				if (context.IndexExists<MappingTestsParentWithSimpleList>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult4 = context.DeleteIndexAsync<MappingTestsParentWithSimpleList>();
					entityResult4.Wait();
				}
				if (context.IndexExists<MappingTestsParentNull>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult5 = context.DeleteIndexAsync<MappingTestsParentNull>();
					entityResult5.Wait();
				}
				if (context.IndexExists<MappingTestsParentWithListNull>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult6 = context.DeleteIndexAsync<MappingTestsParentWithListNull>();
					entityResult6.Wait();
				}
				if (context.IndexExists<MappingTestsParentWithArrayNull>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult7 = context.DeleteIndexAsync<MappingTestsParentWithArrayNull>();
					entityResult7.Wait();
				}
				if (context.IndexExists<MappingTestsParentWithSimpleNullAndNullArrayList>())
				{
					context.AllowDeleteForIndex = true;
					var entityResult8 = context.DeleteIndexAsync<MappingTestsParentWithSimpleNullAndNullArrayList>();
					entityResult8.Wait();
				}
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayList()
		{
			var mappingTestsParent = new MappingTestsParentWithSimpleNullAndNullArrayList
			{
				Calls = 3,
				MappingTestsParentId = 2,
				Call2s="test"
			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentWithSimpleNullAndNullArrayList>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSimpleList()
		{
			var mappingTestsParent = new MappingTestsParentWithSimpleList
			{
				Calls = 3,
				MappingTestsParentId = 2,
				Call2s="test",
				MappingTestsItemIntList = new List<int> { 2, 7, 44, 176},
				MappingTestsItemShortArray =  new short[] { 4,7,89,9}
			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentWithSimpleList>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
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

		[Test]
		public void CreateNewIndexAndMappingForNullNestedChild()
		{
			var mappingTestsParent = new MappingTestsParentNull
			{
				Calls = 3,
				MappingTestsParentId = 2
			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentNull>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingForNestedNullListOfChild()
		{
			var mappingTestsParent = new MappingTestsParentWithListNull
			{
				Calls = 3,
				MappingTestsParentId = 3
			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentWithListNull>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingForNestedNullArrayOfChild()
		{
			var mappingTestsParent = new MappingTestsParentWithArrayNull
			{
				Calls = 3,
				MappingTestsParentId = 3
			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChangesAndInitMappings();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParentWithArrayNull>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingForNestedChildInTwoSteps()
		{
			const string index = "newindextestmappingtwostep";
			IElasticsearchMappingResolver elasticsearchMappingResolver;
			var mappingTestsParent = SetupIndexMappingTests(index, out elasticsearchMappingResolver);

			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.CreateIndex(index);

				Thread.Sleep(1500);
				Assert.IsTrue(context.IndexExists<MappingTestsParent>());
				context.CreateTypeMappingForIndex<MappingTestsParent>(new MappingDefinition {Index = index});

				Thread.Sleep(1500);

				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
				context.SaveChanges();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParent>(mappingTestsParent.MappingTestsParentId);

				Assert.IsNotNull(doc);

				if (context.IndexExists<MappingTestsParent>())
				{
					context.AllowDeleteForIndex = true;
					context.DeleteIndex<MappingTestsParent>();
				}
			}
		}

		/// <summary>
		/// TODO clean up the sleeps...
		/// </summary>
		[Test]
		public void CreateNewIndexAndMappingForNestedChildInTwoStepsWithRouting()
		{
			const string index = "newindextestmappingtwostep";
			IElasticsearchMappingResolver elasticsearchMappingResolver;
			var mappingTestsParent = SetupIndexMappingTests(index, out elasticsearchMappingResolver);
			var routing = new RoutingDefinition {RoutingId = "coolrouting"};
			var config = new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver,true,false,true);
			using (var context = new ElasticsearchContext(ConnectionString, config))
			{
				context.AllowDeleteForIndex = true;
				context.TraceProvider = new ConsoleTraceProvider();
				context.CreateIndex(index);

				Thread.Sleep(1500);
				Assert.IsTrue(context.IndexExists<MappingTestsParent>());
				context.CreateTypeMappingForIndex<MappingTestsParent>(new MappingDefinition { Index = index, RoutingDefinition = routing });

				Thread.Sleep(1500);

				context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId, routing);
				context.SaveChanges();

				Thread.Sleep(1500);
				var doc = context.GetDocument<MappingTestsParent>(mappingTestsParent.MappingTestsParentId, routing);
				Thread.Sleep(1500);

				Assert.IsNotNull(doc);

				context.DeleteIndexType<MappingTestsParent>();
				Thread.Sleep(1500);

				Assert.IsFalse(context.IndexTypeExists<MappingTestsParent>());

				if (context.IndexExists<MappingTestsParent>())
				{
					
					context.DeleteIndex<MappingTestsParent>();
				}
			}
		}

		private static MappingTestsParent SetupIndexMappingTests(string index, out IElasticsearchMappingResolver elasticsearchMappingResolver)
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

			elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof (MappingTestsParent),
				MappingUtils.GetElasticsearchMapping(index));
			return mappingTestsParent;
		}
	}

	public class MappingTestsParentNull
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		[ElasticsearchString(Boost = 1.2, Fields = typeof(FieldDataDef), Index = StringIndex.analyzed)]
		public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

		public MappingTestsChild MappingTestsItem { get; set; }
	}

	public class MappingTestsParent
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store=true)]
		public int Calls { get; set; }

		[ElasticsearchString(Boost = 1.2, Fields = typeof(FieldDataDef), Index = StringIndex.analyzed)]
		public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

		public MappingTestsChild MappingTestsItem { get; set; }
	}

	public class MappingTestsChild
	{
		public int MappingTestsChildId { get; set; }

		[ElasticsearchString(Boost=1.3)]
		public string Description { get; set; }

	}

	public class MappingTestsParentWithList
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		public List<MappingTestsChild> MappingTestsItemList { get; set; }
	}


	public class MappingTestsParentWithListNull
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

		[ElasticsearchString(Boost = 1.4, Fields= typeof(FieldDataDef), Index=StringIndex.analyzed)]
		public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

		public MappingTestsChild[] MappingTestsItemArray { get; set; }
	}

	public class MappingTestsParentWithArrayNull
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		[ElasticsearchString(Boost = 1.9, Fields = typeof(FieldDataDef), Index = StringIndex.analyzed)]
		public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

		public MappingTestsChild[] MappingTestsItemArray { get; set; }
	}

	public class FieldDataDef
	{
		[ElasticsearchString(Index = StringIndex.not_analyzed)]
		public string Raw { get; set; }
	}

	public class MappingTestsParentWithSimpleList
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		public List<int> MappingTestsItemIntList { get; set; }

		public short[] MappingTestsItemShortArray { get; set; }

		[ElasticsearchString(Index=StringIndex.not_analyzed)]
		public string Call2s { get; set; }
	}

	public class MappingTestsParentWithSimpleNullAndNullArrayList
	{
		public int MappingTestsParentId { get; set; }

		[ElasticsearchInteger(Store = true)]
		public int Calls { get; set; }

		public List<int> MappingTestsItemIntList { get; set; }

		public short[] MappingTestsItemShortArray { get; set; }

		[ElasticsearchString(Index = StringIndex.not_analyzed)]
		public string Call2s { get; set; }
	}
}