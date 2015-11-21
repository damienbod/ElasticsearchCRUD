using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.Integration.Test
{
    using Xunit;

    public class DefaultElasticsearchCrudTests : IDisposable
    {
        public DefaultElasticsearchCrudTests()
        {
            SetUp();
        }

        public void Dispose()
        {
            TearDown();
        }

        private List<SkillTestEntity> _entitiesForTests;
        private List<SkillTestEntityTwo> _entitiesForTestsTypeTwo;
        private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
        private readonly AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private const string ConnectionString = "http://localhost:9200";

        private void WaitForDataOrFail()
        {
            if (!_resetEvent.WaitOne(5000))
            {
                throw new Exception("No data received within specified time");
            }
        }

        public void SetUp()
        {
            _entitiesForTests = new List<SkillTestEntity>();
            _entitiesForTestsTypeTwo = new List<SkillTestEntityTwo>();
            // Create a 100 entities
            for (int i = 0; i < 100; i++)
            {
                var entity = new SkillTestEntity
                {
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Description = "A test entity description",
                    Id = i,
                    Name = "cool"
                };

                _entitiesForTests.Add(entity);

                var entityTwo = new SkillTestEntityTwo
                {
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Description = "A test entity cool description",
                    Id = i,
                    Name = "cool"
                };

                _entitiesForTestsTypeTwo.Add(entityTwo);
            }
        }

        public void TearDown()
        {
            _entitiesForTests = null;

            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.AllowDeleteForIndex = true;
                var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

                entityResult.Wait();
                var secondDelete = context.DeleteIndexAsync<SkillTestEntityTwo>();
                secondDelete.Wait();

                var thirdDelete = context.DeleteIndexAsync<TestJsonIgnore>();
                thirdDelete.Wait();
            }
        }

        [Fact]
        public void TestDefaultContextAdd100Entities()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                for (int i = 0; i < 100; i++)
                {
                    context.AddUpdateDocument(_entitiesForTests[i], i);
                }

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(ret.Result.Status, HttpStatusCode.OK);
            }
        }

        [Fact]
        public void TestDefaultContextCount()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                for (int i = 0; i < 7; i++)
                {
                    context.AddUpdateDocument(_entitiesForTests[i], i);
                }

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.Equal(ret.Status, HttpStatusCode.OK);
                long found = 0;
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(300);
                        found = context.Count<SkillTestEntity>();
                        if (found == 7)
                        {
                            _resetEvent.Set();
                        }	
                    }					
                });

                // allow elasticsearch time to update...
                WaitForDataOrFail();

                Assert.Equal(7, found);
            }
        }
        
        [Fact]
        public void TestDefaultContextCountWithNoIndex()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.Count<SkillTestEntityNoIndex>();
                }
            });

            Assert.Equal("ElasticsearchContextCount: Index not found", ex.Message);   
        }

        [Fact]
        public void TestDefaultContextAddEntitySaveChangesAsyncBadUrl()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext("http://locaghghghlhost:9200/", _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    context.AddUpdateDocument(_entitiesForTests[1], 1);
                    var ret = context.SaveChangesAsync();
                    Console.WriteLine(ret.Result.Status);
                }
            });
        }

        [Fact]
        public void TestDefaultContextAddEntitySaveChangesBadUrl()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext("http://locaghghghlhost:9200/", _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    context.AddUpdateDocument(_entitiesForTests[1], 1);
                    context.SaveChanges();
                }
            });
        }

        [Fact]
        public void TestDefaultContextGetEntityAsync()
        {
            const int entityId = 34;
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(ret.Result.Status, HttpStatusCode.OK);

                // Get the entity
                var entityResult = context.GetDocumentAsync<SkillTestEntity>(entityId);
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.OK);
                Assert.Equal(entityResult.Result.PayloadResult.Id, entityId);
                Assert.NotNull(entityResult.Result.PayloadResult);
            }
        }

        [Fact]
        public void TestDefaultContextGetEntity()
        {
            const int entityId = 34;
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.Equal(ret.Status, HttpStatusCode.OK);

                // Get the entity
                var entityResult = context.GetDocument<SkillTestEntity>(entityId);
                Assert.Equal(entityResult.Id, entityId);
            }
        }

        [Fact]
        public void TestDefaultContextSearchExists()
        {
            const int documentId = 34;
            string searchJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";

            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(_entitiesForTests[documentId], documentId);

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.Equal(ret.Status, HttpStatusCode.OK);
    
            }

            Task.Run(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    while (true)
                    {
                        Thread.Sleep(300);
                        var exists = context.SearchExists<SkillTestEntity>(searchJson);
                        if (exists)
                        {
                            _resetEvent.Set();
                        }
                    }
                }
            });

            // allow elasticsearch time to update...
            WaitForDataOrFail();
        }

        [Fact]
        public void TestDefaultContextSearchNotExists()
        {
            const int documentId = 3574474;
            string searchJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";

            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                // Get the entity
                var exists = context.SearchExists<SkillTestEntity>(searchJson);
                Assert.False(exists);
            }
        }

        [Fact]
        public void TestDefaultContextSearchMatchAll()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(_entitiesForTests[34], 34);
                context.AddUpdateDocument(_entitiesForTests[35], 35);
                context.SaveChanges();
            }

            Task.Run(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    while (true)
                    {
                        Thread.Sleep(300);
                        var exists = context.SearchExists<SkillTestEntity>(BuildSearchMatchAll());
                        if (exists)
                        {
                            _resetEvent.Set();
                        }
                    }
                }
            });

            WaitForDataOrFail();
        }

        private string BuildSearchMatchAll()
        {
            var buildJson = new StringBuilder();
            buildJson.AppendLine("{");
            buildJson.AppendLine("\"query\": {");
            buildJson.AppendLine("\"match_all\" : {}");
            buildJson.AppendLine("}");
            buildJson.AppendLine("}");

            return buildJson.ToString();
        }

        [Fact]
        public void TestDefaultContextGetEntityNotFound()
        {
            const int entityId = 39994;

            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    // Get the entity
                    var entityResult = context.GetDocument<SkillTestEntity>(entityId);
                    Assert.Equal(entityResult.Id, entityId);
                }
            });

            Assert.Equal("ElasticSearchContextGet: HttpStatusCode.NotFound", ex.Message);
        }

        [Fact]
        public void TestDefaultContextGetEntityIndexNotFound()
        {
            const int entityId = 39994;
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    // Get the entity
                    var entityResult = context.GetDocument<SkillTestEntityNoIndex>(entityId);
                    Assert.Equal(entityResult.Id, entityId);
                }
            });

            Assert.Equal("ElasticSearchContextGet: HttpStatusCode.NotFound", ex.Message);
        }

        [Fact]
        public void TestDefaultContextGetEntityBadUrl()
        {
            const int entityId = 34;
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext("http://localghghghhost:9200/", _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    context.GetDocument<SkillTestEntity>(entityId);
                }
            }); 
        }

        [Fact]
        public void TestDefaultContextUpdateEntity()
        {
            const int entityId = 34;
            SkillTestEntity resultfromGet;
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                // add a new entity
                context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(ret.Result.Status, HttpStatusCode.OK);

                // Get the entity
                resultfromGet = context.GetDocumentAsync<SkillTestEntity>(entityId).Result.PayloadResult;

            }

            resultfromGet.Name = "updated";
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                // update entity
                context.AddUpdateDocument(resultfromGet, entityId);

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(ret.Result.Status, HttpStatusCode.OK);

                // Get the entity
                var readEntity = context.GetDocumentAsync<SkillTestEntity>(entityId).Result.PayloadResult;
                Assert.Equal(readEntity.Name, resultfromGet.Name);
                Assert.NotEqual(readEntity.Name, _entitiesForTests[entityId].Name);
            }
        }

        [Fact]
        public void TestDefaultContextDeleteEntity()
        {
            const int entityId = 35;
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(_entitiesForTests[entityId], entityId);

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(HttpStatusCode.OK, ret.Result.Status);

                // Get the entity
                var entityResult = context.GetDocumentAsync<SkillTestEntity>(entityId);
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.OK);
                Assert.Equal(entityResult.Result.PayloadResult.Id, entityId);
                Assert.NotNull(entityResult.Result.PayloadResult);

                // Delete the entity
                context.DeleteDocument<SkillTestEntity>(entityId);
                var result = context.SaveChanges();
                Assert.Equal(result.Status, HttpStatusCode.OK);
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.OK);
                Assert.Equal(entityResult.Result.PayloadResult.Id, entityId);
            }
        }


        [Fact]
        public void TestDefaultContextDeleteEntityWhichDoesNotExist()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    // Delete the entity
                    context.DeleteDocument<SkillTestEntity>(6433);
                    context.DeleteDocument<SkillTestEntity>(22222);

                    var task = Task.Run(() => context.SaveChangesAsync());

                    try
                    {
                        task.Wait();
                    }
                    catch (AggregateException ae)
                    {

                        ae.Handle(x =>
                        {
                            if (x is ElasticsearchCrudException) // This is what we expect.
                            {
                                throw x;
                            }
                            return false; // stop.
                        });
                    }
                }
            });
        }

        [Fact]
        public void TestDefaultContextGetEntityWhichDoesNotExist()
        {
            const int entityId = 3004;
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                var entityResult = context.GetDocumentAsync<SkillTestEntity>(entityId);
                entityResult.Wait();
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void TestDefaultContextSaveWithNoChanges()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                var entityResult = context.SaveChangesAsync();
                entityResult.Wait();
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.OK);
            }
        }

        [Fact]
        public void TestDefaultContextDeleteIndex()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                for (int i = 0; i < 100; i++)
                {
                    context.AddUpdateDocument(_entitiesForTests[i], i);
                }

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(ret.Result.Status, HttpStatusCode.OK);
            }

            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AllowDeleteForIndex = true;
                var entityResult = context.DeleteIndexAsync<SkillTestEntityNoIndex>();
                entityResult.Wait();
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void TestDefaultContextDeleteIndexNotFound()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AllowDeleteForIndex = true;
                var entityResult = context.DeleteIndexAsync<SkillTestEntityNoIndex>();
                entityResult.Wait();
                Assert.Equal(entityResult.Result.Status, HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void TestDefaultContextDeleteIndexNotActivated()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    var entityResult = context.DeleteIndexAsync<SkillTestEntity>();

                    try
                    {
                        entityResult.Wait();
                    }
                    catch (AggregateException ae)
                    {

                        ae.Handle(x =>
                        {
                            if (x is ElasticsearchCrudException) // This is what we expect.
                            {
                                throw x;
                            }
                            return false; // stop.
                        });
                    }

                    Assert.Equal(entityResult.Result.Status, HttpStatusCode.OK);
                }
            });
        }

        [Fact]
        public void TestDefaultContextAdd100EntitiesForTwoTypes()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                for (int i = 0; i < 100; i++)
                {
                    context.AddUpdateDocument(_entitiesForTests[i], i);
                    context.AddUpdateDocument(_entitiesForTestsTypeTwo[i], i);
                }

                // Save to Elasticsearch
                var ret = context.SaveChangesAsync();
                Assert.Equal(ret.Result.Status, HttpStatusCode.OK);
            }
        }


        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/plugins/2.0/plugins-delete-by-query.html
        /// bin/plugin install delete-by-query
        /// </summary>
        [Fact]
        public void TestDefaultContextDeleteByQuerySingleDocumentWithId()
        {
            const int documentId = 153;
            string deleteJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";

            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    for (int i = 150; i < 160; i++)
                    {
                        context.AddUpdateDocument(_entitiesForTests[i - 150], i);
                    }
                    // Save to Elasticsearch
                    var ret = context.SaveChanges();

                    // Wait for Elasticsearch to update
                    long foundBefore = 0;

                    Thread.Sleep(1500);
                    foundBefore = context.Count<SkillTestEntity>();


                    Assert.Equal(ret.Status, HttpStatusCode.OK);
                    context.DeleteByQuery<SkillTestEntity>(deleteJson);

                    Thread.Sleep(1500);
                    long foundAfter = context.Count<SkillTestEntity>();

                    Console.WriteLine("found before {0}, after {1}", foundBefore, foundAfter);
                    Assert.InRange(foundAfter, foundBefore, 1000000);
                    context.GetDocument<SkillTestEntity>(documentId);
                }
            });

            Assert.Equal("ElasticSearchContextGet: HttpStatusCode.NotFound", ex.Message);

        }

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/plugins/2.0/plugins-delete-by-query.html
        /// bin/plugin install delete-by-query
        /// </summary>
        [Fact]
        public void TestDefaultContextDeleteByQueryForTwoDocumentsWithIdQuery()
        {
            const int documentId153 = 153;
            const int documentId155 = 155;

            string deleteJson = "{\"query\": { \"ids\": { \"values\":  [\"" + documentId153 + "\", \"" + documentId155 + "\"]  }}}";

            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    for (int i = 150; i < 160; i++)
                    {
                        context.AddUpdateDocument(_entitiesForTests[i - 150], i);
                    }
                    // Save to Elasticsearch
                    var ret = context.SaveChanges();

                    Thread.Sleep(1500);
                    var foundBefore = context.Count<SkillTestEntity>();

                    Assert.Equal(ret.Status, HttpStatusCode.OK);
                    context.DeleteByQuery<SkillTestEntity>(deleteJson);

                    Thread.Sleep(1500);
                    var foundAfter = context.Count<SkillTestEntity>();

                    Console.WriteLine("found before {0}, after {1}", foundBefore, foundAfter);
                    Assert.InRange(foundAfter, foundBefore - 1,100000 );
                    context.GetDocument<SkillTestEntity>(documentId153);
                }
            });

            Assert.Equal("ElasticSearchContextGet: HttpStatusCode.NotFound", ex.Message);
        }
        [Fact]
        public void TestDefaultContextClearCache()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                for (int i = 150; i < 160; i++)
                {
                    context.AddUpdateDocument(_entitiesForTests[i - 150], i);
                }

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.Equal(ret.Status, HttpStatusCode.OK);

                Assert.True(context.IndexClearCache<SkillTestEntity>());
            }
        }

        [Fact]
        public void TestDefaultContextClearCacheForNoIndex()
        {
            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    Assert.True(context.IndexClearCache<SkillTestEntityNoIndex>());
                }
            });

            Assert.Equal("ElasticsearchContextClearCache: Could nor clear cache for index skilltestentitynoindexs", ex.Message);
        }

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/plugins/2.0/plugins-delete-by-query.html
        /// bin/plugin install delete-by-query
        /// </summary>
        [Fact]
        public void TestDefaultContextDeleteByQuerySingleDocumentWithNonExistingId()
        {
            const int documentId = 965428;
            string deleteJson = "{\"query\": { \"term\": { \"_id\": \"" + documentId + "\" }}}";

            var ex = Assert.Throws<ElasticsearchCrudException>(() =>
            {
                using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
                {
                    context.TraceProvider = new ConsoleTraceProvider();
                    for (int i = 150; i < 160; i++)
                    {
                        context.AddUpdateDocument(_entitiesForTests[i - 150], i);
                    }

                    // Save to Elasticsearch
                    var ret = context.SaveChanges();
                    Thread.Sleep(1500);
                    Assert.Equal(ret.Status, HttpStatusCode.OK);
                    context.DeleteByQuery<SkillTestEntity>(deleteJson);
                    Thread.Sleep(1500);
                    context.GetDocument<SkillTestEntity>(documentId);
                }
            });

            Assert.Equal("ElasticSearchContextGet: HttpStatusCode.NotFound", ex.Message);

            
        }

        [Fact]
        public void TestDefaultContextTestJsonIgnore()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var testJsonIgnore = new TestJsonIgnore
                {
                    MyStringArray = new List<string> {"ff", "dd"},
                    BlahBlah = "sss",
                    Id = 3,
                    MyLongArray = new List<long> {23, 4323456, 333332},
                    SkillSingleChildElement = new SkillSingleChildElement {Details = "ss", Id = 3},
                    SkillSingleChildElementList =
                        new List<SkillSingleChildElement> {new SkillSingleChildElement {Details = "ww", Id = 2}}
                };

                context.AddUpdateDocument(testJsonIgnore, testJsonIgnore.Id);

                // Save to Elasticsearch
                context.SaveChanges();

                var ret = context.GetDocument<TestJsonIgnore>(3);
                Assert.Equal(ret.MyLongArray, null);
                Assert.Equal(ret.SkillSingleChildElement, null);
                Assert.Equal(ret.SkillSingleChildElementList, null);
                Assert.Equal(ret.BlahBlahNull, null);
                Assert.Equal(ret.BlahBlah, "sss");
                Assert.Equal(ret.Id, 3);
            }
        }
    }
}
