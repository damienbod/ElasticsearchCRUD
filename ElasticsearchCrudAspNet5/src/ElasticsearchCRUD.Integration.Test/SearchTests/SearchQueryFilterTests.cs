using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Tracing;
using System;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
    public class SearchQueryFilterTests : SetupSearch, IDisposable
    {
        public SearchQueryFilterTests()
        {
            Setup();
        }

        public void Dispose()
        {
            TearDown();
        }

        [Fact]
        public void SearchFilterMatchAllTest()
        {
            var search = new Search { Filter = new Filter(new MatchAllFilter { Boost = 1.1 }) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Fact]
        public void SearchQueryTermFilter()
        {
            var search = new Search { Filter = new Filter(new TermFilter("name", "three") ) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Fact]
        public void SearchQueryTermsFilter()
        {
            var search = new Search { Filter = new Filter(new TermsFilter("name", new List<object> { "one", "three" }) ) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Fact]
        public void SearchQueryExistsFilter()
        {
            var search = new Search { Filter = new Filter(new ExistsFilter("name")) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(1, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
        public void SearchQueryNotFilter()
        {
            var search = new Search
            {
                Filter = new Filter(
                    new NotFilter(new TermFilter("name", "one"))
                )
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
        public void SearchQueryGeoDistanceFilter()
        {
            var search = new Search { Filter = new Filter(new GeoDistanceFilter("location", new GeoPoint(43,43), new DistanceUnitKilometer(1000) )) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.True( items.Description.Contains("invalid radiusMeters"));
            }
        }

        [Fact]
        public void SearchQueryGeoDistanceFilter1SortTest()
        {
            var search = new Search
            {
                Filter =
                    new Filter(
                    new GeoDistanceFilter(
                    "location",
                    new GeoPoint(43, 43),
                    new DistanceUnitMeter(1)
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(0, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
        public void SearchQueryGeoDistanceFilterSmallRange()
        {
            var search = new Search { Filter = new Filter(new GeoDistanceFilter("location", new GeoPoint(43, 43), new DistanceUnitKilometer(1))
            {
                DistanceType= DistanceType.plane,
                OptimizeBbox = OptimizeBbox.none
            }) };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(0, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.HitsResult[0].Source.Id);
            }
        }

        [Fact]
        public void SearchQueryQueryFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new QueryFilter(new MatchAllQuery()))
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
        public void SearchQueryPrefixFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new PrefixFilter("name", "on"))
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(1, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(1, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
        public void SearchQueryScriptFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new ScriptFilter("doc['lift'].value * factor")
                {
                    Params = new List<ScriptParameter>
                    {
                        new ScriptParameter("factor", 1.5)
                    }
                })
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                        IncludeLower = false,
                        IncludeUpper = true
                    }
                )
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(3, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(2, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
        public void SearchQueryRegExpFilter()
        {
            var search = new Search
            {
                Filter = new Filter(new RegExpFilter("name", "o.*")
                {
                  MaxDeterminizedStates = 20000
                })
            };

            using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(1, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(1, items.PayloadResult.Hits.Total);
            }
        }

        [Fact]
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
                Assert.True(context.IndexTypeExists<SearchTest>());
                var items = context.Search<SearchTest>(search);
                Assert.Equal(1, items.PayloadResult.Hits.Total);
            }
        }

    }
}