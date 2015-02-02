using System;
using System.Collections.Generic;
using System.Globalization;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
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
			var search = new Search { Aggs = new List<IAggs>{ new MinMetricAggregation("test_min", "lift")}};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search);
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_min");
				Assert.AreEqual("1.7", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggMinAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new MinMetricAggregation("test_min", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters{SeachType= SeachType.count});
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_min");
				Assert.AreEqual(1.7, aggResult);
			}
		}

		[Test]
		public void SearchAggMinAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new MinMetricAggregation("test_min", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_min");
				Assert.AreEqual(24, (int)aggResult);
			}
		}

		[Test]
		public void SearchAggMaxAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new MaxMetricAggregation("test_max", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_max");
				Assert.AreEqual("2.9", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggMaxAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new MaxMetricAggregation("test_max", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_max");
				Assert.AreEqual(41, (int)aggResult);
			}
		}

		[Test]
		public void SearchAggSumAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new SumMetricAggregation("test_sum", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_sum");
				Assert.AreEqual("16.3", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggSumAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new SumMetricAggregation("test_sum", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_sum");
				Assert.AreEqual(232, (int)aggResult);
			}
		}

		[Test]
		public void SearchAggAvgAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new AvgMetricAggregation("test_avg", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_avg");
				Assert.AreEqual("2.33", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggAvgAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new AvgMetricAggregation("test_avg", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_avg");
				Assert.AreEqual(33, (int)aggResult);
			}
		}

		[Test]
		public void SearchAggStatsAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new StatsMetricAggregation("test_StatsAggregation", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<StatsAggregationsResult>("test_StatsAggregation");
				Assert.AreEqual("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("2.33", Math.Round(aggResult.Avg, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("2.9", Math.Round(aggResult.Max, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("1.7", Math.Round(aggResult.Min, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("16.3", Math.Round(aggResult.Sum, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggStatsAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new StatsMetricAggregation("test_StatsAggregation", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters {SeachType = SeachType.count});
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<StatsAggregationsResult>("test_StatsAggregation");
				Assert.AreEqual("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("33.25", Math.Round(aggResult.Avg, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("41.41", Math.Round(aggResult.Max, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("24.28", Math.Round(aggResult.Min, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("232.76", Math.Round(aggResult.Sum, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggExtendedStatsAggregationNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new ExtendedStatsMetricAggregation("test_ExtendedStatsAggregation", "lift")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<ExtendedStatsAggregationsResult>("test_ExtendedStatsAggregation");
				Assert.AreEqual("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("2.33", Math.Round(aggResult.Avg, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("2.9", Math.Round(aggResult.Max, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("1.7", Math.Round(aggResult.Min, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("16.3", Math.Round(aggResult.Sum, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("0.42", Math.Round(aggResult.StdDeviation, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("39.19", Math.Round(aggResult.SumOfSquares, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("0.18", Math.Round(aggResult.Variance, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggExtendedStatsAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new ExtendedStatsMetricAggregation("test_ExtendedStatsAggregation", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<ExtendedStatsAggregationsResult>("test_ExtendedStatsAggregation");
				Assert.AreEqual("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggValueCountAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new ValueCountMetricAggregation("test_ValueCountAggregation", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_ValueCountAggregation");
				Assert.AreEqual("7", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggValueCountAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new ValueCountMetricAggregation("test_ValueCountAggregation", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_ValueCountAggregation");
				Assert.AreEqual(7, (int)aggResult);
			}
		}

		[Test]
		public void SearchAggValueCountAggregationWithAvgChildNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{ 
					new ValueCountMetricAggregation("test_Value", "lift"),
					new AvgMetricAggregation("aggResultChildAvg", "lift")
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResultValue = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_Value");
				Assert.AreEqual("7", Math.Round(aggResultValue, 2).ToString(CultureInfo.InvariantCulture));
				var aggResultChildAvg = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("aggResultChildAvg");
				Assert.AreEqual("2.33", Math.Round(aggResultChildAvg, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggListAggregationNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new ValueCountMetricAggregation("testvalue", "lift"),
					new SumMetricAggregation("testsum", "lift") 
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResultValue = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testvalue");
				var aggResultSum = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testsum");
				Assert.AreEqual("7", Math.Round(aggResultValue, 2).ToString(CultureInfo.InvariantCulture));
				Assert.AreEqual("16.3", Math.Round(aggResultSum, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Test]
		public void SearchAggPercentilesMetricAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs> { new PercentilesMetricAggregation("test_PercentilesMetricAggregation", "lift") } };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentilesMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.AreEqual(2.1, aggResult.Values["25.0"]);
			}
		}

		[Test]
		public void SearchAggPercentilesMetricAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new PercentilesMetricAggregation("test_PercentilesMetricAggregation", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters {SeachType = SeachType.count});
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentilesMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.AreEqual(29.99, Math.Round(aggResult.Values["25.0"], 2));
			}
		}

		[Test]
		public void SearchAggPercentilesMetricAggregationPrecentsNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new PercentilesMetricAggregation("test_PercentilesMetricAggregation", "lift")
					{
						Percents = new List<double>{20.0, 60.0, 99.0}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentilesMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.AreEqual(2.1, Math.Round(aggResult.Values["20.0"], 2));
			}
		}

		[Test]
		public void SearchAggCardinalityMetricAggregationNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new CardinalityMetricAggregation("testCardinalityMetricAggregation", "lift")
					{
						Rehash= false,
						PrecisionThreshold= 100
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testCardinalityMetricAggregation");
				Assert.AreEqual(2, Math.Round(aggResult, 2));
			}
		}

		[Test]
		public void SearchAggCardinalityMetricAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new CardinalityMetricAggregation("testCardinalityMetricAggregation", "lift")
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testCardinalityMetricAggregation");
				Assert.AreEqual(4, (int)aggResult);
			}
		}

		[Test]
		public void SearchAggPercentileRanksMetricAggregationNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new PercentileRanksMetricAggregation("test_PercentilesMetricAggregation", "lift", new List<double>{20.0, 30.0, 40.0})
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentileRanksAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.AreEqual(100, aggResult.Values["20.0"]);
			}
		}

		[Test]
		public void SearchAggPercentileRanksMetricAggregationScriptNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>{ new PercentileRanksMetricAggregation("test_PercentilesMetricAggregation", "lift", new List<double>{20.0, 30.0, 40.0})
				{
					Script = "_value * times * constant",
					Params = new List<ScriptParameter>
					{
						new ScriptParameter("times", 1.4),
						new ScriptParameter("constant", 10.2)
					}
				}}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.IsTrue(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentileRanksAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.AreEqual(42.92, Math.Round(aggResult.Values["30.0"],2));
			}
		}

	}
}