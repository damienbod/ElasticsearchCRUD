using System;
using System.Threading;
using ElasticsearchCRUD.Integration.Test.SearchTests;
using ElasticsearchCRUD.Model.GeoModel;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	public class SetupSearchAgg
	{
		protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
		protected const string ConnectionString = "http://localhost.fiddler:9200";

		[TestFixtureSetUp]
		public void Setup()
		{
			var doc1 = new SearchAggTest
			{
				Id = 1,
				Details = "This is the details of the document, very interesting",
				Name = "one",
				CircleTest = new GeoShapeCircle { Radius = "100m", Coordinates = new GeoPoint(45, 45) },
				Location = new GeoPoint(45, 45),
				Lift = 2.9,
				DateOfDetails = DateTime.UtcNow.AddDays(-20)
			};

			var doc2 = new SearchAggTest
			{
				Id = 2,
				Details = "Details of the document two, leave it alone",
				Name = "two",
				CircleTest = new GeoShapeCircle { Radius = "50m", Coordinates = new GeoPoint(46, 45) },
				Location = new GeoPoint(46, 45),
				Lift = 2.5,
				DateOfDetails = DateTime.UtcNow.AddDays(-209)
			};
			var doc3 = new SearchAggTest
			{
				Id = 3,
				Details = "This data is different",
				Name = "three",
				CircleTest = new GeoShapeCircle { Radius = "80m", Coordinates = new GeoPoint(37, 42) },
				Location = new GeoPoint(37, 42),
				Lift = 2.1,
				DateOfDetails = DateTime.UtcNow.AddDays(-34)
			};
			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				//context.IndexCreate<SearchAggTest>();
				Thread.Sleep(1000);
				context.AddUpdateDocument(doc1, doc1.Id);
				context.AddUpdateDocument(doc2, doc2.Id);
				context.AddUpdateDocument(doc3, doc3.Id);
				context.SaveChanges();
				Thread.Sleep(1000);
			}
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				//var entityResult = context.DeleteIndexAsync<SearchTest>(); entityResult.Wait();
			}
		}
	}
}