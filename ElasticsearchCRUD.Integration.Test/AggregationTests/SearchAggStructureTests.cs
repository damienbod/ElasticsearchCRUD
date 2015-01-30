using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class SearchAggStructureTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggMinAggregationWithHits()
		{
			var search = new Search { Aggs= new MinAggregation("test_min", "lift")};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchAggMinAggregationNoHits()
		{
			var search = new Search { Aggs = new MinAggregation("test_min", "lift") };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters{SeachType= SeachType.count});
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchAggMinAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new MinAggregation("test_min", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}
	}
}