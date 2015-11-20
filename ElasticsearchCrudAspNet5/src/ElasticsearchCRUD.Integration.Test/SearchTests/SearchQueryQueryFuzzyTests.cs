using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryQueryFuzzyTests : SetupSearch
	{
		[Fact]
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

		[Fact]
		public void SearchQueryFuzzyQueryTwo()
		{
			var search = new Search { Query = new Query(new FuzzyQuery("details", "doxcument") { Fuzziness = 4, MaxExpansions = 50, Boost = 3 }) };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(2, items.PayloadResult.Hits.Total);
			}
		}

	}
}