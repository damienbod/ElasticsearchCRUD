using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class GeoPointTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[TearDown]
		public void TestTearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<GeoPointDto>();
				var entityResult1 = context.DeleteIndexAsync<GeoShapePoint>();
				entityResult.Wait();
				entityResult1.Wait();	
			}
		}

		[Test]
		public void CreateGeoPointMapping()
		{
			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoPointDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoPointDto>());
			}
		}

		[Test]
		public void CreateGeoShapePointMapping()
		{
			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapePoint>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapePoint>());
			}
		}
	}

	public class GeoPointDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoPoint]
		public GeoPoint CityCoordinates { get; set; }
	}

	public class GeoShapePointDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShapePoint]
		public GeoShapePoint ShapeCityCoordinates { get; set; }
	}
}
