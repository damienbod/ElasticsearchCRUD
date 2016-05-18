using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using Xunit;
using System;

namespace ElasticsearchCRUD.Integration.Test.AggregationTests
{
    public class ScriptedMetricAggregationTests : SetupSearchAgg, IDisposable
    {
        public ScriptedMetricAggregationTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
        public void SearchAggScriptedMetricAggregationWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new ScriptedMetricAggregation("topScriptedMetricAggregation", "_agg.transactions.add(doc['lift'].value)")
                    {
                        InitScript= "_agg['transactions'] = []",
                        CombineScript= "script_calculated = 0; for (t in _agg.transactions) { script_calculated += t }; return script_calculated",
                        ReduceScript= "script_calculated = 0; for (a in _aggs) { script_calculated += a }; return script_calculated"
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("topScriptedMetricAggregation");
                Assert.Equal(16.3, aggResult);

            }
        }

        [Fact]
        public void SearchAggScriptedMetricAggregationWithParamsWithNoHits()
        {
            var search = new Search
            {
                Size = 0,
                Aggs = new List<IAggs>
                {
                    new ScriptedMetricAggregation("topScriptedMetricAggregation", "_agg.transactions.add(doc['lift'].value  * _agg.times)")
                    {
                        InitScript= "_agg['transactions'] = []",
                        CombineScript= "script_calculated = 0; for (t in _agg.transactions) { script_calculated += t }; return script_calculated",
                        ReduceScript= "script_calculated = 0; for (a in _aggs) { script_calculated += a }; return script_calculated",
                        Params = new ParamsForScript("_agg")
                        {
                            Params = new List<ScriptParameter>
                            {
                                new ScriptParameter("times", 1.4),
                            }
                        }
                    }
                }
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchAggTest>());
                var items = context.Search<SearchAggTest>(search);
                var aggResult = items.PayloadResult.Aggregations.GetSingleMetricAggregationValue<double>("topScriptedMetricAggregation");
                Assert.Equal(22.82, aggResult);

            }
        }

    }
}
