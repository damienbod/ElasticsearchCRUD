using System;
using System.Threading;
using ElasticsearchCRUD.Model.GeoModel;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class SetupSearchAgg
    {
        protected readonly IElasticsearchMappingResolver ElasticsearchMappingResolver = new ElasticsearchMappingResolver();
        protected const string ConnectionString = "http://localhost.fiddler:9200";

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
                LengthOfSomeThing = 345.4,
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
                LengthOfSomeThing = 289.0,
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
                LengthOfSomeThing = 324.0,
                DateOfDetails = DateTime.UtcNow.AddDays(-34)
            };

            var doc4 = new SearchAggTest
            {
                Id = 4,
                Details = "This data is different from the last one",
                Name = "four",
                CircleTest = new GeoShapeCircle { Radius = "800m", Coordinates = new GeoPoint(34, 42) },
                Location = new GeoPoint(37, 42),
                Lift = 2.1,
                LengthOfSomeThing = 625.0,
                DateOfDetails = DateTime.UtcNow.AddDays(-37)
            };

            var doc5 = new SearchAggTest
            {
                Id = 5,
                Details = "five stuff from the road",
                Name = "five",
                CircleTest = new GeoShapeCircle { Radius = "300m", Coordinates = new GeoPoint(34, 41) },
                Location = new GeoPoint(37, 42),
                Lift = 1.7,
                LengthOfSomeThing = 605.0,
                DateOfDetails = DateTime.UtcNow.AddDays(-47)
            };

            var doc6 = new SearchAggTest
            {
                Id = 6,
                Details = "a lot of things happening now",
                Name = "six",
                CircleTest = new GeoShapeCircle { Radius = "3000m", Coordinates = new GeoPoint(35, 41) },
                Location = new GeoPoint(32, 44),
                Lift = 2.9,
                LengthOfSomeThing = 128.0,
                DateOfDetails = DateTime.UtcNow.AddDays(-46)
            };

            var doc7 = new SearchAggTest
            {
                Id = 7,
                Details = "we made it to seven",
                Name = "seven",
                CircleTest = new GeoShapeCircle { Radius = "4000m", Coordinates = new GeoPoint(35, 40) },
                Location = new GeoPoint(34, 43),
                Lift = 2.1,
                LengthOfSomeThing = 81,
                DateOfDetails = DateTime.UtcNow.AddDays(-46)
            };
            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                if (!context.IndexExists<SearchAggTest>())
                { 
                    context.IndexCreate<SearchAggTest>();
                    Thread.Sleep(1200);
                    context.AddUpdateDocument(doc1, doc1.Id);
                    context.AddUpdateDocument(doc2, doc2.Id);
                    context.AddUpdateDocument(doc3, doc3.Id);
                    context.AddUpdateDocument(doc4, doc4.Id);
                    context.AddUpdateDocument(doc5, doc5.Id);
                    context.AddUpdateDocument(doc6, doc6.Id);
                    context.AddUpdateDocument(doc7, doc7.Id);
                    context.SaveChanges();
                    Thread.Sleep(1500);
                }			
            }
        }

        public void TearDown()
        {
            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.AllowDeleteForIndex = true;
                var entityResult = context.DeleteIndexAsync<SearchAggTest>(); entityResult.Wait();
            }
        }
    }
}