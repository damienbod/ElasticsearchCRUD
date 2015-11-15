using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class GeoPointAndGeoShapeTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

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
				var entityResult4 = context.DeleteIndexAsync<GeoShapeMultiPointDto>();
				var entityResult5 = context.DeleteIndexAsync<GeoShapeMultiLineStringDto>();
				var entityResult6 = context.DeleteIndexAsync<GeoShapeMultiPolygonDto>();
				var entityResult7 = context.DeleteIndexAsync<GeoShapeGeometryCollectionDto>();
				var entityResult8 = context.DeleteIndexAsync<GeoShapeEnvelopeDto>();
				var entityResult9 = context.DeleteIndexAsync<GeoShapeCircleDto>();

				entityResult.Wait();
				entityResult1.Wait();
				entityResult2.Wait();
				entityResult3.Wait();
				entityResult4.Wait();
				entityResult5.Wait();
				entityResult6.Wait();
				entityResult7.Wait();
				entityResult8.Wait();
				entityResult9.Wait();
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
				var result = context.SearchById<GeoPointDto>(1);
				Assert.AreEqual(geoPointDto.CityCoordinates.Count, result.CityCoordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapePointMapping()
		{
			var geoShapePointDto = new GeoShapePointDto
			{
				ShapeCityCoordinates = new GeoShapePoint
				{
					Coordinates =  new GeoPoint(45, 45)
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
				var result = context.SearchById<GeoShapePointDto>(1);
				Assert.AreEqual(geoShapePointDto.ShapeCityCoordinates.Coordinates.Count, result.ShapeCityCoordinates.Coordinates.Count);
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
				var result = context.SearchById<GeoShapeLineStringDto>(1);
				Assert.AreEqual(geoShapeLineStringDto.ShapeLineCoordinates.Coordinates.Count, result.ShapeLineCoordinates.Coordinates.Count);
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
				var result = context.SearchById<GeoShapePolygonDto>(1);
				Assert.AreEqual(geoShapePolygonDto.Coordinates.Coordinates.Count, result.Coordinates.Coordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapeMultiPointMapping()
		{
			var geoShapeLineStringDto = new GeoShapeMultiPointDto
			{
				Coordinates = new GeoShapeMultiPoint
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
				context.IndexCreate<GeoShapeMultiPointDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeMultiPointDto>());

				context.AddUpdateDocument(geoShapeLineStringDto, geoShapeLineStringDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeMultiPointDto>());
				var result = context.SearchById<GeoShapeMultiPointDto>(1);
				Assert.AreEqual(geoShapeLineStringDto.Coordinates.Coordinates.Count, result.Coordinates.Coordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapeMultiLineStringMapping()
		{
			var geoShapeMultiLineStringDto = new GeoShapeMultiLineStringDto
			{
				Coordinates = new GeoShapeMultiLineString
				{
					Coordinates = new List<List<GeoPoint>>
					{
						new List<GeoPoint>
						{
							new GeoPoint(45, 45),
							new GeoPoint(46, 45)
						},
						new List<GeoPoint>
						{
							new GeoPoint(145, 45),
							new GeoPoint(146, 42)
						}
					}
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapeMultiLineStringDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeMultiLineStringDto>());

				context.AddUpdateDocument(geoShapeMultiLineStringDto, geoShapeMultiLineStringDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeMultiLineStringDto>());
				var result = context.SearchById<GeoShapeMultiLineStringDto>(1);
				Assert.AreEqual(geoShapeMultiLineStringDto.Coordinates.Coordinates.Count, result.Coordinates.Coordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapeMultiPolygonMapping()
		{
			var geoShapeMultiPolygonDto = new GeoShapeMultiPolygonDto
			{
				Coordinates = new GeoShapeMultiPolygon
				{
					Coordinates = new List<List<List<GeoPoint>>>
					{
						new List<List<GeoPoint>>
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
					}
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapeMultiPolygonDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeMultiPolygonDto>());

				context.AddUpdateDocument(geoShapeMultiPolygonDto, geoShapeMultiPolygonDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeMultiPolygonDto>());
				var result = context.SearchById<GeoShapeMultiPolygonDto>(1);
				Assert.AreEqual(geoShapeMultiPolygonDto.Coordinates.Coordinates.Count, result.Coordinates.Coordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapeEnvelopeMapping()
		{
			var geoShapeEnvelopeDto = new GeoShapeEnvelopeDto
			{
				Coordinates = new GeoShapeEnvelope
				{
					Coordinates = new List<GeoPoint>
					{
						// [ [-45.0, 45.0], [45.0, -45.0] ]

						new GeoPoint(-45, 45),
						new GeoPoint(45, -45)
					}
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapeEnvelopeDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeEnvelopeDto>());

				context.AddUpdateDocument(geoShapeEnvelopeDto, geoShapeEnvelopeDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeEnvelopeDto>());
				var result = context.SearchById<GeoShapeEnvelopeDto>(1);
				Assert.AreEqual(geoShapeEnvelopeDto.Coordinates.Coordinates.Count, result.Coordinates.Coordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapeCircleMapping()
		{
			var geoShapeCircleDto = new GeoShapeCircleDto
			{
				CircleTest = new GeoShapeCircle
				{
					Coordinates = new GeoPoint(45, 45),
					Radius="100m"
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapeCircleDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeCircleDto>());

				context.AddUpdateDocument(geoShapeCircleDto, geoShapeCircleDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeCircleDto>());
				var result = context.SearchById<GeoShapeCircleDto>(1);
				Assert.AreEqual(geoShapeCircleDto.CircleTest.Coordinates.Count, result.CircleTest.Coordinates.Count);
			}
		}

		[Test]
		public void CreateGeoShapeGeometryCollectionMapping()
		{
			var geoShapeGeometryCollectionDto = new GeoShapeGeometryCollectionDto
			{
				Coordinates = new GeoShapeGeometryCollection
				{
					Geometries = new List<object>
					{
						new GeoShapeMultiLineString
						{
							Coordinates =  new List<List<GeoPoint>>
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
						new GeoShapePoint
						{
							Coordinates =  new GeoPoint(45, 45)
						},
						new GeoShapeLineString
						{
							Coordinates = new List<GeoPoint>
							{
								new GeoPoint(45, 45),
								new GeoPoint(46, 45)
							}
						}
					}
				},
				Id = "1",
				Name = "test",
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<GeoShapeGeometryCollectionDto>();

				Thread.Sleep(1500);
				Assert.IsNotNull(context.IndexExists<GeoShapeGeometryCollectionDto>());

				context.AddUpdateDocument(geoShapeGeometryCollectionDto, geoShapeGeometryCollectionDto.Id);
				context.SaveChanges();
				Thread.Sleep(1500);
				Assert.AreEqual(1, context.Count<GeoShapeGeometryCollectionDto>());
				var result = context.SearchById<GeoShapeGeometryCollectionDto>(1);
				Assert.AreEqual(geoShapeGeometryCollectionDto.Coordinates.Geometries.Count, result.Coordinates.Geometries.Count);
			}
		}

	}

	public class GeoPointDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoPoint(FieldDataPrecision="1cm")]
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

	public class GeoShapeMultiPointDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape(Tree= GeoShapeTree.quadtree)]
		public GeoShapeMultiPoint Coordinates { get; set; }
	}

	public class GeoShapeMultiLineStringDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape(Tree = GeoShapeTree.quadtree)]
		public GeoShapeMultiLineString Coordinates { get; set; }
	}

	public class GeoShapeMultiPolygonDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape(Precision = "100m")]
		public GeoShapeMultiPolygon Coordinates { get; set; }
	}

	public class GeoShapeGeometryCollectionDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape(Precision = "100m")]
		public GeoShapeGeometryCollection Coordinates { get; set; }
	}

	public class GeoShapeEnvelopeDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape]
		public GeoShapeEnvelope Coordinates { get; set; }
	}

	public class GeoShapeCircleDto
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[ElasticsearchGeoShape]
		public GeoShapeCircle CircleTest { get; set; }
	}

}
