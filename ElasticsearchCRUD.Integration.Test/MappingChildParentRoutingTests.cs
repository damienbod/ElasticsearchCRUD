using System.ComponentModel.DataAnnotations;
using System.Net;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class MappingChildParentRoutingTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[Test]
		public void DeleteChildTypeFromExistingIndex()
		{
			CreateIndex();

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var result = context.DeleteIndexType<MappingChildParentRoutingTestsLevel3>();
				Assert.IsTrue(result);
			}
		}


		private void CreateIndex()
		{
			var doc = new MappingChildParentRoutingTestsLevel1
			{
				MappingChildParentRoutingTestsLevel1Id = 1,
				Level2 = new MappingChildParentRoutingTestsLevel2()
				{
					MappingChildParentRoutingTestsLevel2Id = 2,
					Level3 = new MappingChildParentRoutingTestsLevel3()
					{
						MappingChildParentRoutingTestsLevel3Id = 3
					}
				}
			};

			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(MappingChildParentRoutingTestsLevel1),
			MappingUtils.GetElasticsearchMapping(new IndexTypeDescription("masterindex", "level1")));
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(MappingChildParentRoutingTestsLevel2),
				MappingUtils.GetElasticsearchMapping(new IndexTypeDescription("masterindex", "level2")));
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(MappingChildParentRoutingTestsLevel3),
				MappingUtils.GetElasticsearchMapping(new IndexTypeDescription("masterindex", "level3")));

			using ( var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration( _elasticsearchMappingResolver,true,true,true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(doc, doc.MappingChildParentRoutingTestsLevel1Id);

				var ret = context.SaveChangesAndInitMappings();
				// Save to Elasticsearch
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}
		}
	}

	public class MappingChildParentRoutingTestsLevel1
	{
		public short MappingChildParentRoutingTestsLevel1Id { get; set; }

		public MappingChildParentRoutingTestsLevel2 Level2 { get; set; }
	}

	public class MappingChildParentRoutingTestsLevel2
	{
		[Key]
		public short MappingChildParentRoutingTestsLevel2Id { get; set; }

		public MappingChildParentRoutingTestsLevel3 Level3 { get; set; }
	}

	public class MappingChildParentRoutingTestsLevel3
	{
		[Key]
		public short MappingChildParentRoutingTestsLevel3Id { get; set; }
	}
}
