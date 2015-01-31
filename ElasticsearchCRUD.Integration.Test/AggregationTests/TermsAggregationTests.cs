using System;
using System.Collections.Generic;
using System.Globalization;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class TermsAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggMinAggregationWithNoHits()
		{
			var search = new Search { Aggs = new List<IAggs> { new TermsAggregation("test_min", "lift") } };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleValueMetric<double>("test_min");
				Assert.AreEqual("2.1", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}
