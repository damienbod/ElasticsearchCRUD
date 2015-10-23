using System.Linq;
using System.Text;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;
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
				if (context.IndexExists<MappingTypeAnalyzerTest>())
				{
					context.DeleteIndex<MappingTypeAnalyzerTest>();
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

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTypeAllTest>(indexDefinition);

				context.AddUpdateDocument(mappingTypeAll, mappingTypeAll.Id);
				context.SaveChanges();

				Thread.Sleep(1500);


				var doc = context.Search<MappingTypeAllTest>(BuildSearchById(1));
				Assert.GreaterOrEqual(doc.PayloadResult.Hits.HitsResult.First().Id.ToString(), "1");
			}
		}

		[Test]
		public void CreateNewIndexAndMappingWithSourceDisabled()
		{
			var indexDefinition = new IndexDefinition {IndexSettings = {NumberOfShards = 3, NumberOfReplicas = 1}};
			indexDefinition.Mapping.Source.Enabled = false;

			var mappingTypeAll = new MappingTypeSourceTest
			{
				Id = 1,
				DescStoreFalse = "non",
				DescStoreTrue = "yes",
				DescThreeNoDef = "three"
			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTypeSourceTest>(indexDefinition);

				context.AddUpdateDocument(mappingTypeAll, mappingTypeAll.Id);
				context.SaveChanges();

				Thread.Sleep(1500);

				var doc = context.Search<MappingTypeSourceTest>(BuildSearchById(1));
				Assert.GreaterOrEqual(doc.PayloadResult.Hits.HitsResult.First().Id.ToString(), "1");
				Assert.IsNull(doc.PayloadResult.Hits.HitsResult.First().Source);
			}
		}

		private string BuildSearchById(object childId)
		{
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"filtered\": {");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"term\": {\"_id\": " + childId + "}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}

		[Test]
		public void CreateNewIndexAndMappingWithAnalyzer()
		{
			var indexDefinition = new IndexDefinition { IndexSettings = { NumberOfShards = 3, NumberOfReplicas = 1 } };
			indexDefinition.Mapping.All.Enabled = false;
			indexDefinition.Mapping.Analyzer = new MappingAnalyzer { Path = "myanalyzer" };

			var mappingTypeAll = new MappingTypeAnalyzerTest
			{
				Id = 1,
				SomeText = "I think search engines are great",
				MyAnalyzer= "whitespace"

			};

			using (
				var context = new ElasticsearchContext(ConnectionString,
					new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTypeAnalyzerTest>(indexDefinition);

				context.AddUpdateDocument(mappingTypeAll, mappingTypeAll.Id);
				context.SaveChanges();

				Thread.Sleep(1500);

				var doc = context.Search<MappingTypeAnalyzerTest>(BuildSearchById(1));
				Assert.GreaterOrEqual(doc.PayloadResult.Hits.HitsResult.First().Id.ToString(), "1");
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
		[ElasticsearchInteger(Store = true)]
		public long Id { get; set; }

		[ElasticsearchString(Store = true)]
		public string DescStoreTrue { get; set; }

		[ElasticsearchString(Store = false)]
		public string DescStoreFalse { get; set; }

		public string DescThreeNoDef { get; set; }
	}

	public class MappingTypeAnalyzerTest
	{
		[ElasticsearchInteger(Store = true)]
		public long Id { get; set; }

		public string SomeText { get; set; }

		public string MyAnalyzer { get; set; }
	}

}
