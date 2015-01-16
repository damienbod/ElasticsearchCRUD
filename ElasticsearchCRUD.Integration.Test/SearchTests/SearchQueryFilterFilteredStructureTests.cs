using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryFilterFilteredStructureTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[Test]
		public void SearchFilteredQueryFilterMatchAll()
		{
			var search = new Search
			{
				Query = new Query(
					new Filtered(
						new Filter(
							new MatchAllFilter { Boost = 1.1 }
						)
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchFilteredQueryFilterMatchAllQueryMatchAll()
		{
			var search = new Search
			{
				Query = new Query(
					new Filtered( 
						new Filter( new MatchAllFilter { Boost = 1.1 } )) { Query = new Query(new MatchAllQuery())}		
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryConstantScoreQueryWithFilter()
		{
			var search = new Search { Query = new Query(new ConstantScoreQuery(new RangeFilter("id"){GreaterThan = "1"}) { Boost = 2.0 }) };

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchAllSortFieldIdDesc()
		{
			var search = new Search
			{
				Query = new Query(new MatchAllQuery()),
				Sort = new SortHolder(
					new List<ISort>
					{
						new SortStandard("id")
						{
							Order=OrderEnum.desc, Mode=SortMode.avg, Missing = SortMissing._last, UnmappedType="int"					
						}
					}
				) { TrackScores = true }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchAllGeoSortFieldAsc()
		{
			var search = new Search
			{
				Query = new Query(new MatchAllQuery()),
				Sort = new SortHolder(
					new List<ISort>
					{
						new SortGeoDistance("location", DistanceUnitEnum.km)
						{
							GeoPoint = new GeoPoint(46, 46),
							Order=OrderEnum.asc, Mode = SortModeGeo.max				
						}
					}
				) { TrackScores = true }
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchAllGeoSortMultipleFieldsAsc()
		{
			var search = new Search
			{
				Query = new Query(new MatchAllQuery()),
				Sort = new SortHolder(
					new List<ISort>
					{
						new SortGeoDistance("location", DistanceUnitEnum.km)
						{
							GeoPoints = new List<GeoPoint>
							{
								new GeoPoint(46, 46),
								new GeoPoint(49, 46),
							},
							Order=OrderEnum.asc, Mode = SortModeGeo.max				
						}
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchAllSortScript()
		{
			var search = new Search
			{
				Query = new Query(new MatchAllQuery()),
				Sort = new SortScript("doc['lift'].value * factor")
				{
					Order = OrderEnum.asc,
					ScriptType= "number",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("factor", 1.5)
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<SearchTest>(); entityResult.Wait();
			}
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			var doc1 = new SearchTest
			{
				Id = 1,
				Details = "This is the details of the document, very interesting",
				Name = "one",
				CircleTest = new GeoShapeCircle { Radius = "100m", Coordinates = new GeoPoint(45, 45) },
				Location = new GeoPoint(45, 45),
				Lift = 2.9
			};

			var doc2 = new SearchTest
			{
				Id = 2,
				Details = "Details of the document two, leave it alone",
				Name = "two",
				CircleTest = new GeoShapeCircle { Radius = "50m", Coordinates = new GeoPoint(46, 45) },
				Location = new GeoPoint(46, 45),
				Lift = 2.5
			};
			var doc3 = new SearchTest
			{
				Id = 3,
				Details = "This data is different",
				Name = "three",
				CircleTest = new GeoShapeCircle { Radius = "80m", Coordinates = new GeoPoint(37, 42) },
				Location = new GeoPoint(37, 42),
				Lift = 2.1
			};
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<SearchTest>();
				Thread.Sleep(1000);
				context.AddUpdateDocument(doc1, doc1.Id);
				context.AddUpdateDocument(doc2, doc2.Id);
				context.AddUpdateDocument(doc3, doc3.Id);
				context.SaveChanges();
				Thread.Sleep(1000);
			}
		}
	}
}
