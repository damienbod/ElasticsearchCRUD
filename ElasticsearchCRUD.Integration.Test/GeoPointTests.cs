using System.Collections.Generic;
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
				var entityResult1 = context.DeleteIndexAsync<GeoShapePointDto>();
				var entityResult2 = context.DeleteIndexAsync<GeoShapeLineStringDto>();
				var entityResult3 = context.DeleteIndexAsync<GeoShapePolygonDto>();
		
				entityResult.Wait();
				entityResult1.Wait();
				entityResult2.Wait();
				entityResult3.Wait();
			}
		}

		[Test]
		public void CreateGeoPointMapping()
		{
			var geoPointDto = new GeoPointDto
			{
				CityCoordinates = new GeoPoint(45, 45),
				Id = "1",
				Name="test"
			};
			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoPointDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoPointDto>());

				context.AddUpdateDocument(geoPointDto, geoPointDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoPointDto>());
			}
		}

		[Test]
		public void CreateGeoShapePointMapping()
		{
			var geoShapePointDto = new GeoShapePointDto
			{
				ShapeCityCoordinates = new GeoShapePoint
				{
					Coordinate =  new GeoPoint(45, 45)
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapePointDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapePointDto>());

				context.AddUpdateDocument(geoShapePointDto, geoShapePointDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapePointDto>());
			}
		}

		[Test]
		public void CreateGeoShapeLineStringMapping()
		{
			var geoShapeLineStringDto = new GeoShapeLineStringDto
			{
				ShapeLineCoordinates = new GeoShapeLineString
				{
					Coordinates = new List<GeoPoint>
					{
						new GeoPoint(45, 45),
						new GeoPoint(46, 45)
					}
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapeLineStringDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeLineStringDto>());

				context.AddUpdateDocument(geoShapeLineStringDto, geoShapeLineStringDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeLineStringDto>());
			}
		}

		[Test]
		public void CreateGeoShapePolygonMapping()
		{
			var geoShapePolygonDto = new GeoShapePolygonDto
			{
				Coordinates = new GeoShapePolygon
				{
					Coordinates = new List<List<GeoPoint>>
					{
						// [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0]
						new List<GeoPoint>
						{
							new GeoPoint(100, 0),
							new GeoPoint(101, 0),
							new GeoPoint(101, 1),
							new GeoPoint(100, 1),
							new GeoPoint(100, 0)
						},
						//  [ [100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2] ]
						new List<GeoPoint>
						{
							new GeoPoint(100.2, 0.2),
							new GeoPoint(100.8, 0.2),
							new GeoPoint(100.8, 0.8),
							new GeoPoint(100.2, 0.8),
							new GeoPoint(100.2, 0.2)
						}
					}
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapePolygonDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapePolygonDto>());

				context.AddUpdateDocument(geoShapePolygonDto, geoShapePolygonDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1,context.Count<GeoShapePolygonDto>());
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

		[ElasticsearchGeoShape]
		public GeoShapePoint ShapeCityCoordinates { get; set; }
	}

	public class GeoShapeLineStringDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape]
		public GeoShapeLineString ShapeLineCoordinates { get; set; }
	}

	public class GeoShapePolygonDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape(Precision = "100m")]
		public GeoShapePolygon Coordinates { get; set; }
	}
}
