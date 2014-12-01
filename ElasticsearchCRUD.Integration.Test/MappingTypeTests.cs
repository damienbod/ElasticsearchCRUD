using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{

	[TestFixture]
	public class MappingTypeTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				if (context.IndexExists<MappingTypeAllTest>())
				{					
					context.DeleteIndex<MappingTypeAllTest>();
				}
				if (context.IndexExists<MappingTypeSourceTest>())
				{
					context.DeleteIndex<MappingTypeSourceTest>();
				}
			}
			
		}

		[Test]
		public void CreateNewIndexAndMappingWithAllDisabled()
		{
			var indexDefinition = new IndexDefinition {IndexSettings = {NumberOfShards = 3, NumberOfReplicas = 1}};
			indexDefinition.Mapping.All.Enabled = false;

			var mappingTypeAll = new MappingTypeAllTest
			{
				Id = 1,
				DescIncludeInAllFalse = "non",
				DescIncludeInAllTrue = "yes",
				DescThreeNoDef = "three"
			};

			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTypeAllTest>(indexDefinition);

				context.AddUpdateDocument(mappingTypeAll, mappingTypeAll.Id);
				context.SaveChanges();

				Thread.Sleep(1000);

				// TODO do a Match word search on the _all field
				var doc = context.GetDocument<MappingTypeAllTest>(1);
				Assert.GreaterOrEqual(doc.Id, 1);
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSourceDisabled()
		{
			var indexDefinition = new IndexDefinition { IndexSettings = { NumberOfShards = 3, NumberOfReplicas = 1 } };
			indexDefinition.Mapping.Source.Enabled = false;

			var mappingTypeAll = new MappingTypeSourceTest
			{
				Id = 1,
				DescStoreFalse = "non",
				DescStoreTrue = "yes",
				DescThreeNoDef = "three"
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTypeSourceTest>(indexDefinition);

				context.AddUpdateDocument(mappingTypeAll, mappingTypeAll.Id);
				context.SaveChanges();

				Thread.Sleep(1000);
				var doc = context.GetDocument<MappingTypeSourceTest>(1);
				Assert.GreaterOrEqual(doc.Id, 1);
			}
		}
	}

	public class MappingTypeAllTest
	{
		public long Id { get; set; }

		[ElasticsearchString(IncludeInAll = true)]
		public string DescIncludeInAllTrue { get; set; }

		[ElasticsearchString(IncludeInAll = false)]
		public string DescIncludeInAllFalse { get; set; }

		public string DescThreeNoDef { get; set; }
	}

	public class MappingTypeSourceTest
	{
		[ElasticsearchInteger(Store=true)]
		public long Id { get; set; }

		[ElasticsearchString(Store = true)]
		public string DescStoreTrue { get; set; }

		[ElasticsearchString(Store = false)]
		public string DescStoreFalse { get; set; }

		public string DescThreeNoDef { get; set; }
	}
}
