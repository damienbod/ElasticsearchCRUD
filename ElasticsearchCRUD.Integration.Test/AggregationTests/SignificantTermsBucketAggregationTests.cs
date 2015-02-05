using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	[TestFixture]
	public class SignificantTermsBucketAggregationTests : SetupSearchAgg
	{
		[Test]
		public void SearchAggSignificantTermsBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
				Assert.AreEqual(7, aggResult.DocCount);
			}
		}

		[Test]
		public void SearchAggTermsBucketAggregationWithSubAggSignificantTermsBucketAggregationWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new TermsBucketAggregation("termsDetails", "details")
					{
						Aggs = new List<IAggs>
						{
							new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
							{
								InformationRetrievalRetrieval = new Jlh()
							}
						}
					}					
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var termsAggResult = items.PayloadResult.Aggregations.GetComplexValue<TermsBucketAggregationsResult>("termsDetails");
				var significantAggResult = termsAggResult.Buckets[0].GetSubAggregationsFromJTokenName<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
				Assert.AreEqual(4, significantAggResult.Buckets[0].BgCount);
			}
		}

		[Test]
		public void SearchAggSignificantTermsBucketAggregationPropertiesSetWithNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new SignificantTermsBucketAggregation("testSignificantTermsBucketAggregation", "details")
					{
						BackgroundFilter= new TermFilter("name", "one"),
						InformationRetrievalRetrieval = new MutualInformation
						{
							BackgroundIsSuperset = false,
							IncludeNegatives = false
						},
						ExecutionHint= ExecutionHint.global_ordinals
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<SignificantTermsBucketAggregationsResult>("testSignificantTermsBucketAggregation");
				Assert.AreEqual(7, aggResult.DocCount);
			}
		}

	}
}
