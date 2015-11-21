using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	public class SearchQueryQueryMatchTests : SetupSearch, IDisposable
    {
        public SearchQueryQueryMatchTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

		[Fact]
		public void SearchQueryMatchAllTest()
		{
			var search = new Search { Query = new Query(new MatchAllQuery { Boost = 1.1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchQueryMatchTest()
		{
			var search = new Search { Query = new Query(new MatchQuery("name", "document one") { Boost = 1.1, Fuzziness = 1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchQueryMatchPhaseTest()
		{
			var search = new Search { Query = new Query(new MatchPhaseQuery("name", "one") { Analyzer = LanguageAnalyzers.German, Slop = 1, Operator = Operator.and, CutoffFrequency = 0.2, ZeroTermsQuery = ZeroTermsQuery.none, Boost = 1.1, Fuzziness = 1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchQueryMatchPhasePrefixTest()
		{
			var search = new Search { Query = new Query(new MatchPhasePrefixQuery("name", "one") { MaxExpansions = 500, Boost = 1.1, MinimumShouldMatch = "50%", Fuzziness = 1 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

		[Fact]
		public void SearchQueryMultiMatchAllTest()
		{
			var search = new Search { Query = new Query(new MultiMatchQuery("document") { MultiMatchType = MultiMatchType.most_fields, TieBreaker = 0.5, Fields = new List<string> { "name", "details" } }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
			}
		}

	}
}