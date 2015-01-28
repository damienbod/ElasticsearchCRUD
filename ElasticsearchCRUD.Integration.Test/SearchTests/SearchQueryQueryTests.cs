using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryQueryTests : SetupSearch
	{
		[Test]
		public void SearchQueryTermQuery()
		{
			var search = new Search { Query = new Query(new TermQuery("name", "one") { Boost = 2.0 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsQuery()
		{
			var search = new Search { Query = new Query(new TermsQuery("name", new List<object> { "one" }) { Boost = 2.0 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryTermsQueryTwoResults()
		{
			var search = new Search { Query = new Query(new TermsQuery("name", new List<object> { "one", "two" }) { Boost = 2.0, MinimumShouldMatch = "1" }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[1].Source.Id);
			}
		}

		[Test]
		public void SearchQueryRangeQuery()
		{
			var search = new Search { Query = new Query(
				new RangeQuery("id")
				{
					GreaterThanOrEqualTo = "2", 
					LessThan = "3", 
					LessThanOrEqualTo = "2", 
					GreaterThan = "1", 
					Boost = 2.0,
					IncludeLower = false,
					IncludeUpper = false
				}) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryBoolQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new BoolQuery
					{
						Must = new List<IQuery>
						{
							new RangeQuery("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1" }
						},
						MustNot = new List<IQuery>
						{
							new RangeQuery("id") {GreaterThan="34"}
						},
						Should = new List<IQuery>
						{
							new TermQuery("name", "two")
						},
						Boost=2.0,
						DisableCoord= false,
						MinimumShouldMatch= "2"
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryBoostingQuery()
		{
			var search = new Search { Query = new Query(new BoostingQuery(new MatchAllQuery(), new TermQuery("name", "two"), 3.0 )) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryDisMaxQueryQuery()
		{
			var search = new Search { Query = new Query(
					new DisMaxQuery
					{
						Boost=2,
						TieBreaker=0.5, 
						Queries = new List<IQuery>
						{
							new TermQuery("name", "one"),
							new RangeQuery("id")
							{
								GreaterThanOrEqualTo = "1",
								Boost = 2
							}

						}
					}
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}
		
		[Test]
		public void SearchQueryConstantScoreQueryWithQuery()
		{
			var search = new Search { Query = new Query(new ConstantScoreQuery(new TermQuery("name", "two")){Boost = 2.0}) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}
		
		[Test]
		public void SearchQueryQueryStringQuery()
		{
			var search = new Search { Query = new Query(new QueryStringQuery("one document")) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryQueryStringQueryAllProperties()
		{
			var search = new Search { Query = new Query(new QueryStringQuery("one document")
			{
				AllowLeadingWildcard= true,
				Analyzer=LanguageAnalyzers.German,
				AnalyzeWildcard=false,
				AutoGeneratePhraseQueries= false,
				Boost=2.1,
				DefaultField="_all",
				DefaultOperator= QueryDefaultOperator.OR,
				EnablePositionIncrements= true,
				Fuzziness=0.5,
				FuzzyMaxExpansions= 49,
				FuzzyPrefixLength=0,
				Lenient=false,
				Locale="ROOT"
			}) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryQueryStringQueryMultipleFields()
		{
			var search = new Search
			{
				Query = new Query(new QueryStringQuery("one document")
				{
					Fields = new List<string> { "name", "details"},
					TieBreaker= 1,
					UseDisMax= true
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryIdsQuery()
		{
			var search = new Search
			{
				Query = new Query(new IdsQuery(new List<object> { 1, 2 })
				{
					Type = "searchtest"
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryPrefixQuery()
		{
			var search = new Search
			{
				Query = new Query(new PrefixQuery("name", "on")
				{
					Boost = 2.0
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryRegExpQuery()
		{
			var search = new Search
			{
				Query = new Query(new RegExpQuery("name", "o.*")
				{
					Boost=1.7,
					Flags = RegExpFlags.INTERSECTION
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryWildcardQuery()
		{
			var search = new Search
			{
				Query = new Query(new WildcardQuery("name", "o*")
				{
					Boost = 2.0
				})
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryGeoShapeQuery()
		{
			var search = new Search
			{
				Query = new Query(
						new GeoShapeQuery("circletest",
							new GeoShapePolygon
							{
								Coordinates = new List<List<GeoPoint>>
								{
									new List<GeoPoint>
									{
										new GeoPoint(40,40),
										new GeoPoint(50,40),
										new GeoPoint(50,50),
										new GeoPoint(40,50),
										new GeoPoint(40,40)
									}
								}
							}
						)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQuerySimpleQueryStringQuery()
		{
			var search = new Search
			{
				Query = new Query(new SimpleQueryStringQuery("doc*")
				{
					Analyzer=DefaultAnalyzers.Snowball,
					DefaultOperator= QueryDefaultOperator.OR,
					Flags = SimpleQueryFlags.ALL,
					Lenient=true,
					LowercaseExpandedTerms=true,
					Fields = new List<string> { "details"}
				}) 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}
	}
}