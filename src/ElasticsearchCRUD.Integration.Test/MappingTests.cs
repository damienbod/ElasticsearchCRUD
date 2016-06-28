using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using Xunit;
using System;

namespace ElasticsearchCRUD.Integration.Test
{
    public class MappingTests : IDisposable
    {
        private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
        private const string ConnectionString = "http://localhost:9200";

        [Fact]
        public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayList()
        {
            var mappingTestsParent = new MappingTestsParentWithSimpleNullAndNullArrayList
            {
                Calls = 3,
                MappingTestsParentId = 2,
                Call2s="test"
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentWithSimpleNullAndNullArrayList>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);

                Thread.Sleep(1500);
                context.IndexClose("mappingtestsparentwithsimplenullandnullarraylists");
                Thread.Sleep(1500);
                context.IndexOpen("mappingtestsparentwithsimplenullandnullarraylists");
                Thread.Sleep(1500);
                var result = context.IndexOptimize("mappingtestsparentwithsimplenullandnullarraylists", new OptimizeParameters{NumberOfShards=3, Flush=true});

                Assert.InRange(result.PayloadResult.Shards.Successful, 2, 1000000);
            }
        }

        [Fact]
        public void OpenIndexWhichDoesNotExist()
        {            
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (
                    var context = new ElasticsearchContext(ConnectionString,
                        new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
                {
                    context.IndexClose("help");
                }
            });

            Assert.Equal("CloseOpenIndexAsync: HttpStatusCode.NotFound index does not exist", ex.Message);
        }

        [Fact]
        public void CloseIndexWhichDoesNotExist()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
                {
                    context.IndexClose("help");
                }
            });

            Assert.Equal("CloseOpenIndexAsync: HttpStatusCode.NotFound index does not exist", ex.Message);
        }

        [Fact]
        public void OptimizeIndexWhichDoesNotExist()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
                {
                    var result = context.IndexOptimize("help", new OptimizeParameters { NumberOfShards = 3, Flush = true });
                }
            });

            Assert.Equal("IndexOptimizeAsync: HttpStatusCode.NotFound index does not exist", ex.Message);            
        }

        [Fact]
        public void CreateNewIndexAndMappingWithSimpleList()
        {
            var mappingTestsParent = new MappingTestsParentWithSimpleList
            {
                Calls = 3,
                MappingTestsParentId = 2,
                Call2s="test",
                MappingTestsItemIntList = new List<int> { 2, 7, 44, 176},
                MappingTestsItemShortArray =  new short[] { 4,7,89,9}
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentWithSimpleList>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNestedChild()
        {
            var mappingTestsParent = new MappingTestsParent
            {
                Calls = 3,
                MappingTestsParentId = 2,
                MappingTestsItem = new MappingTestsChild
                {
                    Description = "Hello nested",
                    MappingTestsChildId = 5
                }
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParent>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNestedListOfChild()
        {
            var mappingTestsParent = new MappingTestsParentWithList
            {
                Calls = 3,
                MappingTestsParentId = 3,
                MappingTestsItemList = new List<MappingTestsChild>
                {
                    new MappingTestsChild
                    {
                        Description = "Hello nested",
                        MappingTestsChildId = 6
                    },
                    new MappingTestsChild
                    {
                        Description = "Hello nested item in list",
                        MappingTestsChildId = 7
                    }
                }

            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentWithList>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNestedArrayOfChild()
        {
            var mappingTestsParent = new MappingTestsParentWithArray
            {
                Calls = 3,
                MappingTestsParentId = 3,
                MappingTestsItemArray = new[]
                {
                    new MappingTestsChild
                    {
                        Description = "Hello nested",
                        MappingTestsChildId = 6
                    },
                    new MappingTestsChild
                    {
                        Description = "Hello nested item in list",
                        MappingTestsChildId = 7
                    }
                }

            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentWithArray>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNullNestedChild()
        {
            var mappingTestsParent = new MappingTestsParentNull
            {
                Calls = 3,
                MappingTestsParentId = 2
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentNull>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNestedNullListOfChild()
        {
            var mappingTestsParent = new MappingTestsParentWithListNull
            {
                Calls = 3,
                MappingTestsParentId = 3
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentWithListNull>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNestedNullArrayOfChild()
        {
            var mappingTestsParent = new MappingTestsParentWithArrayNull
            {
                Calls = 3,
                MappingTestsParentId = 3
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParentWithArrayNull>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void CreateNewIndexAndMappingForNestedChildInTwoSteps()
        {
            const string index = "newindextestmappingtwostep";
            IElasticsearchMappingResolver elasticsearchMappingResolver;
            var mappingTestsParent = SetupIndexMappingTests(index, out elasticsearchMappingResolver);

            using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver)))
            {

                context.TraceProvider = new ConsoleTraceProvider();
                context.IndexCreate(index, new IndexSettings{BlocksWrite =true, NumberOfShards = 7});

                Thread.Sleep(1500);
                Assert.True(context.IndexExists<MappingTestsParent>());
                context.IndexCreateTypeMapping<MappingTestsParent>(new MappingDefinition { Index = index });

                Thread.Sleep(1500);

                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId);
                context.SaveChanges();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParent>(mappingTestsParent.MappingTestsParentId);

                Assert.NotNull(doc);

                if (context.IndexExists<MappingTestsParent>())
                {
                    context.AllowDeleteForIndex = true;
                    context.DeleteIndex<MappingTestsParent>();
                }
            }
        }

        /// <summary>
        /// TODO clean up the sleeps...
        /// </summary>
        [Fact]
        public void CreateNewIndexAndMappingForNestedChildInTwoStepsWithRouting()
        {
            const string index = "newindextestmappingtwostep";
            IElasticsearchMappingResolver elasticsearchMappingResolver;
            var mappingTestsParent = SetupIndexMappingTests(index, out elasticsearchMappingResolver);
            var routing = new RoutingDefinition {RoutingId = "coolrouting"};
            var config = new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver,true,false,true);
            using (var context = new ElasticsearchContext(ConnectionString, config))
            {
                context.AllowDeleteForIndex = true;
                context.TraceProvider = new ConsoleTraceProvider();
                context.IndexCreate(index);

                Thread.Sleep(1500);
                Assert.True(context.IndexExists<MappingTestsParent>());
                context.IndexCreateTypeMapping<MappingTestsParent>(new MappingDefinition { Index = index, RoutingDefinition = routing });

                Thread.Sleep(1500);

                context.AddUpdateDocument(mappingTestsParent, mappingTestsParent.MappingTestsParentId, routing);
                context.SaveChanges();

                Thread.Sleep(1500);
                var doc = context.GetDocument<MappingTestsParent>(mappingTestsParent.MappingTestsParentId, routing);
                Thread.Sleep(1500);

                Assert.NotNull(doc);

                context.DeleteIndex(index);

                Thread.Sleep(1500);

                Assert.False(context.IndexTypeExists<MappingTestsParent>());

                if (context.IndexExists<MappingTestsParent>())
                {
                    
                    context.DeleteIndex<MappingTestsParent>();
                }
            }
        }

        [Fact]
        public void UpdateIndexSettings()
        {
            const string index = "newindextestmappingtwostep";
            IElasticsearchMappingResolver elasticsearchMappingResolver;
            var mappingTestsParent = SetupIndexMappingTests(index, out elasticsearchMappingResolver);

            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver)))
            {

                context.TraceProvider = new ConsoleTraceProvider();
                context.IndexCreate(index, new IndexSettings { BlocksWrite = true, NumberOfShards = 7 });

                //context.IndexClose(index);
                context.IndexUpdateSettings(new IndexUpdateSettings {  NumberOfReplicas = 2 }, index );
                //context.IndexOpen(index);

                Thread.Sleep(1500);
                Assert.True(context.IndexExists<MappingTestsParent>());
                

                if (context.IndexExists<MappingTestsParent>())
                {
                    context.AllowDeleteForIndex = true;
                    context.DeleteIndex<MappingTestsParent>();
                }
            }
        }

        [Fact]
        public void UpdateIndexSettingsClosedIndex()
        {
            const string index = "newindextestmappingtwostep";
            IElasticsearchMappingResolver elasticsearchMappingResolver;
            var mappingTestsParent = SetupIndexMappingTests(index, out elasticsearchMappingResolver);

            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver)))
            {

                context.TraceProvider = new ConsoleTraceProvider();
                context.IndexCreate(index, new IndexSettings { BlocksWrite = true, NumberOfShards = 7 });

                Thread.Sleep(1500);
                context.IndexClose(index);
                Thread.Sleep(1500);
                
                context.IndexOpen(index);

                Thread.Sleep(1500);
                Assert.True(context.IndexExists<MappingTestsParent>());


                if (context.IndexExists<MappingTestsParent>())
                {
                    context.AllowDeleteForIndex = true;
                    context.DeleteIndex<MappingTestsParent>();
                }
            }
        }

        [Fact]
        public void UpdateIndexSettingsGlobal()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchMappingResolver()))
            {
                if (!context.IndexExists<MappingTestsParent>())
                {
                    context.IndexCreate<MappingTestsParent>();
                }
                
                context.TraceProvider = new ConsoleTraceProvider();
                var result = context.IndexUpdateSettings(new IndexUpdateSettings { NumberOfReplicas = 1 });
                context.AllowDeleteForIndex = true;
                context.DeleteIndex<MappingTestsParent>();
                Assert.Equal("completed", result.PayloadResult);
            }
        }

        private static MappingTestsParent SetupIndexMappingTests(string index, out IElasticsearchMappingResolver elasticsearchMappingResolver)
        {
            var mappingTestsParent = new MappingTestsParent
            {
                Calls = 3,
                MappingTestsParentId = 2,
                MappingTestsItem = new MappingTestsChild
                {
                    Description = "Hello nested",
                    MappingTestsChildId = 5
                }
            };

            elasticsearchMappingResolver = new ElasticsearchMappingResolver();
            elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof (MappingTestsParent),
                MappingUtils.GetElasticsearchMapping(index));
            return mappingTestsParent;
        }

        public void Dispose()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                if (context.IndexExists<MappingTestsParent>())
                {
                    context.AllowDeleteForIndex = true;
                    context.DeleteIndex<MappingTestsParent>();
                }
                if (context.IndexExists<MappingTestsParentWithList>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult2 = context.DeleteIndexAsync<MappingTestsParentWithList>();
                    entityResult2.Wait();
                }
                if (context.IndexExists<MappingTestsParentWithArray>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult3 = context.DeleteIndexAsync<MappingTestsParentWithArray>();
                    entityResult3.Wait();
                }
                if (context.IndexExists<MappingTestsParentWithSimpleList>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult4 = context.DeleteIndexAsync<MappingTestsParentWithSimpleList>();
                    entityResult4.Wait();
                }
                if (context.IndexExists<MappingTestsParentNull>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult5 = context.DeleteIndexAsync<MappingTestsParentNull>();
                    entityResult5.Wait();
                }
                if (context.IndexExists<MappingTestsParentWithListNull>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult6 = context.DeleteIndexAsync<MappingTestsParentWithListNull>();
                    entityResult6.Wait();
                }
                if (context.IndexExists<MappingTestsParentWithArrayNull>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult7 = context.DeleteIndexAsync<MappingTestsParentWithArrayNull>();
                    entityResult7.Wait();
                }
                if (context.IndexExists<MappingTestsParentWithSimpleNullAndNullArrayList>())
                {
                    context.AllowDeleteForIndex = true;
                    var entityResult8 = context.DeleteIndexAsync<MappingTestsParentWithSimpleNullAndNullArrayList>();
                    entityResult8.Wait();
                }
            }
        }
    }

    public class MappingTestsParentNull
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        [ElasticsearchString(Boost = 1.2, Fields = typeof(FieldDataDef), Index = StringIndex.analyzed)]
        public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

        public MappingTestsChild MappingTestsItem { get; set; }
    }

    public class MappingTestsParent
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store=true)]
        public int Calls { get; set; }

        [ElasticsearchString(Boost = 1.2, Fields = typeof(FieldDataDef), Index = StringIndex.analyzed)]
        public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

        public MappingTestsChild MappingTestsItem { get; set; }
    }

    public class MappingTestsChild
    {
        public int MappingTestsChildId { get; set; }

        [ElasticsearchString(Boost=1.3)]
        public string Description { get; set; }

    }

    public class MappingTestsParentWithList
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        public List<MappingTestsChild> MappingTestsItemList { get; set; }
    }


    public class MappingTestsParentWithListNull
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        public List<MappingTestsChild> MappingTestsItemList { get; set; }
    }

    public class MappingTestsParentWithArray
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        [ElasticsearchString(Boost = 1.4, Fields= typeof(FieldDataDef), Index=StringIndex.analyzed)]
        public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

        public MappingTestsChild[] MappingTestsItemArray { get; set; }
    }

    public class MappingTestsParentWithArrayNull
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        [ElasticsearchString(Boost = 1.9, Fields = typeof(FieldDataDef), Index = StringIndex.analyzed)]
        public string DescriptionBothAnayzedAndNotAnalyzed { get; set; }

        public MappingTestsChild[] MappingTestsItemArray { get; set; }
    }

    public class FieldDataDef
    {
        [ElasticsearchString(Index = StringIndex.not_analyzed, Analyzer = "english")]
        public string Raw { get; set; }

        [ElasticsearchString( Analyzer = LanguageAnalyzers.English)]
        public string EnglishAnalzed { get; set; }
    }

    public class MappingTestsParentWithSimpleList
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        public List<int> MappingTestsItemIntList { get; set; }

        public short[] MappingTestsItemShortArray { get; set; }

        [ElasticsearchString(Index=StringIndex.not_analyzed)]
        public string Call2s { get; set; }
    }

    public class MappingTestsParentWithSimpleNullAndNullArrayList
    {
        public int MappingTestsParentId { get; set; }

        [ElasticsearchInteger(Store = true)]
        public int Calls { get; set; }

        public List<int> MappingTestsItemIntList { get; set; }

        public short[] MappingTestsItemShortArray { get; set; }

        [ElasticsearchString(Index = StringIndex.analyzed, Analyzer = DefaultAnalyzers.Whitespace)]
        public string Call2s { get; set; }
    }


}