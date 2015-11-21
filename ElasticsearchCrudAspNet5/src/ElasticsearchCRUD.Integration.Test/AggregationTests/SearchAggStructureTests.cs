using System;
using System.Collections.Generic;
using System.Globalization;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
	public class SearchAggStructureTests : SetupSearchAgg, IDisposable
    {
	    public SearchAggStructureTests()
	    {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
		public void SearchAggMinAggregationWithHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new MinMetricAggregation("test_min", "lift")}};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search);
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_min");
				Assert.Equal("1.7", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
		public void SearchAggMinAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new MinMetricAggregation("test_min", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters{SeachType= SeachType.count});
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_min");
				Assert.Equal(1.7, aggResult);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_min");
				Assert.Equal(24, (int)aggResult);
			}
		}

		[Fact]
		public void SearchAggMaxAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new MaxMetricAggregation("test_max", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_max");
				Assert.Equal("2.9", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_max");
				Assert.Equal(41, (int)aggResult);
			}
		}

		[Fact]
		public void SearchAggSumAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new SumMetricAggregation("test_sum", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_sum");
				Assert.Equal("16.3", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_sum");
				Assert.Equal(232, (int)aggResult);
			}
		}

		[Fact]
		public void SearchAggAvgAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new AvgMetricAggregation("test_avg", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_avg");
				Assert.Equal("2.33", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_avg");
				Assert.Equal(33, (int)aggResult);
			}
		}

		[Fact]
		public void SearchAggStatsAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new StatsMetricAggregation("test_StatsAggregation", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<StatsMetricAggregationsResult>("test_StatsAggregation");
				Assert.Equal("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("2.33", Math.Round(aggResult.Avg, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("2.9", Math.Round(aggResult.Max, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("1.7", Math.Round(aggResult.Min, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("16.3", Math.Round(aggResult.Sum, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters {SeachType = SeachType.count});
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<StatsMetricAggregationsResult>("test_StatsAggregation");
				Assert.Equal("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("33.25", Math.Round(aggResult.Avg, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("41.41", Math.Round(aggResult.Max, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("24.28", Math.Round(aggResult.Min, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("232.76", Math.Round(aggResult.Sum, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<ExtendedStatsMetricAggregationsResult>("test_ExtendedStatsAggregation");
				Assert.Equal("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("2.33", Math.Round(aggResult.Avg, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("2.9", Math.Round(aggResult.Max, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("1.7", Math.Round(aggResult.Min, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("16.3", Math.Round(aggResult.Sum, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("0.42", Math.Round(aggResult.StdDeviation, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("39.19", Math.Round(aggResult.SumOfSquares, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("0.18", Math.Round(aggResult.Variance, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<ExtendedStatsMetricAggregationsResult>("test_ExtendedStatsAggregation");
				Assert.Equal("7", Math.Round(aggResult.Count, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
		public void SearchAggValueCountAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs>{ new ValueCountMetricAggregation("test_ValueCountAggregation", "lift") }};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_ValueCountAggregation");
				Assert.Equal("7", Math.Round(aggResult, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_ValueCountAggregation");
				Assert.Equal(7, (int)aggResult);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResultValue = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("test_Value");
				Assert.Equal("7", Math.Round(aggResultValue, 2).ToString(CultureInfo.InvariantCulture));
				var aggResultChildAvg = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("aggResultChildAvg");
				Assert.Equal("2.33", Math.Round(aggResultChildAvg, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResultValue = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testvalue");
				var aggResultSum = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testsum");
				Assert.Equal("7", Math.Round(aggResultValue, 2).ToString(CultureInfo.InvariantCulture));
				Assert.Equal("16.3", Math.Round(aggResultSum, 2).ToString(CultureInfo.InvariantCulture));
			}
		}

		[Fact]
		public void SearchAggPercentilesMetricAggregationNoHits()
		{
			var search = new Search { Aggs = new List<IAggs> { new PercentilesMetricAggregation("test_PercentilesMetricAggregation", "lift") } };

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentilesMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.Equal(2.1, aggResult.Values["25.0"]);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters {SeachType = SeachType.count});
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentilesMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.Equal(29.99, Math.Round(aggResult.Values["25.0"], 2));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentilesMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.Equal(2.1, Math.Round(aggResult.Values["20.0"], 2));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testCardinalityMetricAggregation");
				Assert.Equal(4, Math.Round(aggResult, 2));
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("testCardinalityMetricAggregation");
				Assert.Equal(4, (int)aggResult);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentileRanksMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.Equal(100, aggResult.Values["20.0"]);
			}
		}

		[Fact]
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
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<PercentileRanksMetricAggregationsResult>("test_PercentilesMetricAggregation");
				Assert.Equal(42.92, Math.Round(aggResult.Values["30.0"],2));
			}
		}

		[Fact]
		public void SearchAggGeoBoundsMetricAggregationNoHits()
		{
			var search = new Search
			{
				Aggs = new List<IAggs>
				{
					new GeoBoundsMetricAggregation("testGeoBoundsMetricAggregation", "location")
					{
						WrapLongitude = true
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				Assert.True(context.IndexTypeExists<SearchAggTest>());
				var items = context.Search<SearchAggTest>(search, new SearchUrlParameters { SeachType = SeachType.count });
				var aggResult = items.PayloadResult.Aggregations.GetComplexValue<GeoBoundsMetricAggregationsResult>("testGeoBoundsMetricAggregation");
				
				Assert.Equal(32.0, aggResult.BoundsTopLeft[0]);
				Assert.Equal(45.0, aggResult.BoundsTopLeft[1]);
			}
		}
		

	}
}