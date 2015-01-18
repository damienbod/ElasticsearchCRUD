using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryQueryTests : SetupSearch
	{
		[Test]
		public void SearchQueryMatchAllTest()
		{
			var search = new Search { Query = new Query(new MatchAllQuery { Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchTest()
		{
			var search = new Search { Query = new Query(new MatchQuery("name", "document one") { Boost = 1.1, Fuzziness = 1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchPhaseTest()
		{
			var search = new Search { Query = new Query(new MatchPhaseQuery("name", "one") { Analyzer = LanguageAnalyzers.German, Slop = 1, Operator = Operator.and, CutoffFrequency = 0.2, ZeroTermsQuery = ZeroTermsQuery.none, Boost = 1.1, Fuzziness = 1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMatchPhasePrefixTest()
		{
			var search = new Search { Query = new Query(new MatchPhasePrefixQuery("name", "one") { MaxExpansions = 500, Boost = 1.1, MinimumShouldMatch = "50%", Fuzziness = 1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryMultiMatchAllTest()
		{
			var search = new Search { Query = new Query(new MultiMatch("document") { MultiMatchType = MultiMatchType.most_fields, TieBreaker = 0.5, Fields = new List<string> { "name", "details" } }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

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
		public void SearchQueryFuzzyQuery()
		{
			var search = new Search { Query = new Query(new FuzzyQuery("details", "dsta")) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryFuzzyQueryTwo()
		{
			var search = new Search { Query = new Query(new FuzzyQuery("details", "doxcument"){Fuzziness=4, MaxExpansions=50, Boost = 3}) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFuzzyLikeThisFieldQuery()
		{
			var search = new Search { Query = new Query(
					new FuzzyLikeThisFieldQuery("details","dats")
					{
						Analyzer=LanguageAnalyzers.Irish,
						Boost=2,
						Fuzziness=0.6,
						IgnoreTf = false,
						MaxQueryTerms=24,
						PrefixLength=2
					}
				) 
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Test]
		public void SearchQueryFuzzyLikeThisQuery()
		{
			var search = new Search
			{
				Query = new Query(
					new FuzzyLikeThisQuery("dats")
					{
						Fields = new List<string>{"details", "name"},
						Analyzer = LanguageAnalyzers.Irish,
						Boost = 2,
						Fuzziness = 0.6,
						IgnoreTf = false,
						MaxQueryTerms = 24,
						PrefixLength = 2
					}
					)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
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
	}
}