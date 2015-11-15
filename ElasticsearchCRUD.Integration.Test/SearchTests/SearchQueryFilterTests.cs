using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
    [TestFixture]
    public class SearchQueryFilterTests : SetupSearch
    {
        [Test]
        public void SearchFilterMatchAllTest()
        {
            var search = new Search { Filter = new Filter(new MatchAllFilter { Boost = 1.1 }) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Test]
        public void SearchQueryTermFilter()
        {
            var search = new Search { Filter = new Filter(new TermFilter("name", "three") { Cache = false }) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Test]
        public void SearchQueryTermsFilter()
        {
            var search = new Search { Filter = new Filter(new TermsFilter("name", new List<object> { "one", "three" }) { Execution = ExecutionMode.@bool }) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.HitsResult[1].Source.Id);
                Assert.AreEqual(1, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Test]
        public void SearchQueryTermsFilterExecutionModeAnd()
        {
            //var search = new Search { Filter = new Filter(new TermsFilter("name", new List<object> { "one", "three" }) { Execution = ExecutionMode.and }) };
            var search = new Search { Filter = new Filter(new TermsFilter("name", new List<object> { "one", "three" }) ) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(2, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryRangeFilter()
        {
            var search = new Search { Filter = new Filter(
                new RangeFilter("id")
                {
                    GreaterThanOrEqualTo = 2, 
                    LessThan = 3, 
                    LessThanOrEqualTo = 2, 
                    GreaterThan = 1,
                    IncludeLower = false,
                    IncludeUpper = false
                }) 
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
        public void SearchQueryBoolFilter()
        {
            var search = new Search
            {
                Filter = new Filter(
                    new BoolFilter( new RangeFilter("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1" })
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
        public void SearchQueryBoolFilterTwo()
        {
            var search = new Search
            {
                Filter = new Filter(
                    new BoolFilter
                    {
                        Must = new List<IFilter>
                        {
                            new RangeFilter("id") { GreaterThanOrEqualTo = "2", LessThan = "3", LessThanOrEqualTo = "2", GreaterThan = "1" }
                        },
                        MustNot = new List<IFilter>
                        {
                            new RangeFilter("id") {GreaterThan="34"}
                        },
                        Cache = false,
                        Should = new List<IFilter>
                        {
                            new TermFilter("name", "two")
                        }
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
        public void SearchQueryExistsFilter()
        {
            var search = new Search { Filter = new Filter(new ExistsFilter("name")) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryAndFilter()
        {
            var search = new Search { Filter = new Filter(
                    new AndFilter(
                        new List<IFilter>
                        {
                            new ExistsFilter("name"),
                            new TermFilter("name", "one")
                        }
                    )
                    {
                        Cache = false
                    }
                ) 
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
        public void SearchQueryOrFilter()
        {
            var search = new Search
            {
                Filter = new Filter(
                    new OrFilter(
                        new List<IFilter>
                        {
                            new TermFilter("name", "one"),
                            new TermFilter("name", "two")
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
        public void SearchQueryNotFilter()
        {
            var search = new Search
            {
                Filter = new Filter(
                    new NotFilter(new TermFilter("name", "one"))
                    {
                        Cache = false
                    }
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
        public void SearchQueryGeoDistanceFilter()
        {
            var search = new Search { Filter = new Filter(new GeoDistanceFilter("location", new GeoPoint(43,43), new DistanceUnitKilometer(1000) )) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryGeoDistanceFilter0Range()
        {
            var search = new Search { Filter = 
                new Filter(
                    new GeoDistanceFilter( 
                        "location", 
                        new GeoPoint(43, 43), 
                        new DistanceUnitMeter(0)
                    )
                )
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(0, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryGeoDistanceFilter0SortTest()
        {
            var search = new Search
            {
                Filter =
                    new Filter(
                    new GeoDistanceFilter(
                    "location",
                    new GeoPoint(43, 43),
                    new DistanceUnitMeter(0)
                    )
                ),
                Sort = new SortHolder(
                    new List<ISort>
                    {
                        new SortGeoDistance("location", DistanceUnitEnum.m, new GeoPoint(46,46))
                        {						
                            Order = OrderEnum.asc,
                            Mode = SortModeGeo.max	
                        },
                    }

                )
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(0, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryGeoDistanceFilterSmallRange()
        {
            var search = new Search { Filter = new Filter(new GeoDistanceFilter("location", new GeoPoint(43, 43), new DistanceUnitKilometer(1))
            {
                Cache= false,
                DistanceType= DistanceType.plane,
                OptimizeBbox = OptimizeBbox.none
            }) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(0, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryIdsFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new IdsFilter(new List<object> { 1, 2 })
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
        public void SearchFilteredQueryFilterLimit()
        {
            var search = new Search
            {
                Query = new Query(
                    new Filtered(
                        new Filter(
                            new LimitFilter(1))
                        )
                    )
                
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Test]
        public void SearchQueryQueryFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new QueryFilter(new MatchAllQuery())
                {
                    Cache=true
                })
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryPrefixFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new PrefixFilter("name", "on")
                {
                    Cache = false
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
        public void SearchQueryMissingFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new MissingFilter("notknown")
                {
                    Existence = true,
                    NullValue=true
                })
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryGeoBoundingBoxFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new GeoBoundingBoxFilter("location", new GeoPoint(46,46),new GeoPoint(44,44) )
                {
                    Type= GeoBoundingBoxFilterType.memory
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
        public void SearchQueryScriptFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new ScriptFilter("doc['lift'].value * factor")
                {
                    Params = new List<ScriptParameter>
                    {
                        new ScriptParameter("factor", 1.5)
                    },
                    Cache=false
                })
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryGeoDistanceRangeFilter()
        {
            var search = new Search
            {
                Filter = new Filter(
                    new GeoDistanceRangeFilter(
                        "location", 
                        new GeoPoint(43, 43), 
                        new DistanceUnitKilometer(1), 
                        new DistanceUnitKilometer(1000)
                    )
                    {
                        GreaterThan="1km",
                        GreaterThanOrEqualTo = "2km",
                        LessThan = "1000km",
                        LessThanOrEqualTo = "1000km",
                        Cache=false,
                        IncludeLower = false,
                        IncludeUpper = true
                    }
                )
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(3, items.PayloadResult.Hits.Total);
            }
        }

        [Test]
        public void SearchQueryGeoPolygonFilter()
        {
            var search = new Search
            {
                Filter = new Filter( 
                    new GeoPolygonFilter("location", 
                        new List<GeoPoint>
                        {
                            new GeoPoint(40,40),
                            new GeoPoint(50,40),
                            new GeoPoint(50,50),
                            new GeoPoint(40,50),
                            new GeoPoint(40,40)
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
        public void SearchQueryGeoShapeFilter()
        {
            var search = new Search
            {
                Filter = new Filter(
                        new GeoShapeFilter("circletest", 
                            new GeoShapePolygon
                            {
                                Coordinates	= new List<List<GeoPoint>>
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
        public void SearchQueryRegExpFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new RegExpFilter("name", "o.*")
                {
                // EL 2.0 bug, query not work when set
                  //MaxDeterminizedStates = 20000
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
        public void SearchQueryRegExpFilterTwo()
        {
            var search = new Search
            {
                Filter = new Filter(new RegExpFilter("name", "o.*")
                {
                    Name = "tee", Flags= RegExpFlags.COMPLEMENT
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
        public void SearchQueryGeoHashFilter()
        {
            var search = new Search { Filter = new Filter(
                new GeohashCellFilter(
                    "location", 
                    new GeoPoint(43, 43), 
                    3, 
                    true)) 
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.IsTrue(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.AreEqual(1, items.PayloadResult.Hits.Total);
            }
        }

    }
}