using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.OneToN
{
    [TestFixture]
    public class OneToNEntitiesWithChildDocumentsTestUserDefinedRouting
    {
        private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
        private const bool SaveChildObjectsAsWellAsParent = true;
        private const bool ProcessChildDocumentsAsSeparateChildIndex = true;
        private const bool UserDefinedRouting = true;

        private const string ConnectionString = "http://localhost:9200";

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            TestCreateCompletelyNewIndex(new ConsoleTraceProvider());
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.AllowDeleteForIndex = true;
                var entityResult = context.DeleteIndexAsync<ParentDocumentUserDefinedRouting>();
                entityResult.Wait();
            }
        }

        [Fact]
        public void TestGetChildItemTestParentDoesNotExist()
        {
            const int parentId = 22;
            // This could return NOT FOUND 404 or OK 200. It all depends is the routing matches the same shard. It does not search for the exact parent

            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                var roundTripResult = context.GetDocument<ChildDocumentLevelTwoUserDefinedRouting>(71, new RoutingDefinition { ParentId = parentId, RoutingId = 7 });

                var childDocs = context.Search<ChildDocumentLevelTwoUserDefinedRouting>(BuildSearchForChildDocumentsWithIdAndParentType(parentId, "childdocumentleveloneuserdefinedrouting"));
                Assert.IsNotNull(childDocs.PayloadResult.Hits.HitsResult.First(t => t.Id.ToString() == "71"));
                Assert.AreEqual(71, roundTripResult.Id);
            }
        }

        // {
        //  "query": {
        //	"term": { "_parent": "parentdocument#7" }
        //  }
        // }
        private string BuildSearchForChildDocumentsWithIdAndParentType(object parentId, string parentDocumentType)
        {
            var buildJson = new StringBuilder();
            buildJson.AppendLine("{");
            buildJson.AppendLine("\"query\": {");
            buildJson.AppendLine("\"term\": {\"_parent\": \"" + parentDocumentType + "#" + parentId + "\"}");
            buildJson.AppendLine("}");
            buildJson.AppendLine("}");

            return buildJson.ToString();
        }

        [Fact]
        public void TestAddUpdateWithExistingMappingsTest()
        {
            var parentDocument2 = ParentDocument2();

            using (var context = new ElasticsearchContext(ConnectionString,
                new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent,
                    ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(parentDocument2, parentDocument2.Id);

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.AreEqual(ret.Status, HttpStatusCode.OK);

                Thread.Sleep(1500);

                var roundTripResult = context.GetDocument<ParentDocumentUserDefinedRouting>(parentDocument2.Id);
                var roundTripResultChildDocumentLevelOne =
                    context.GetDocument<ChildDocumentLevelOneUserDefinedRouting>(parentDocument2.ChildDocumentLevelOne.First().Id,
                        new RoutingDefinition {ParentId = parentDocument2.Id, RoutingId = parentDocument2.Id});

                var roundTripResultChildDocumentLevelTwo =
                    context.GetDocument<ChildDocumentLevelTwoUserDefinedRouting>(
                        parentDocument2.ChildDocumentLevelOne.First().ChildDocumentLevelTwo.Id,
                        new RoutingDefinition
                        {
                            ParentId = parentDocument2.ChildDocumentLevelOne.First().Id,
                            RoutingId = parentDocument2.Id
                        });

                Assert.AreEqual(parentDocument2.Id, roundTripResult.Id);
                Assert.AreEqual(parentDocument2.ChildDocumentLevelOne.First().Id, roundTripResultChildDocumentLevelOne.Id);
                Assert.AreEqual(parentDocument2.ChildDocumentLevelOne.First().ChildDocumentLevelTwo.Id,
                    roundTripResultChildDocumentLevelTwo.Id);

                var childDocs =
                    context.Search<ChildDocumentLevelTwoUserDefinedRouting>(
                        BuildSearchForChildDocumentsWithIdAndParentType(parentDocument2.ChildDocumentLevelOne.FirstOrDefault().Id,
                            "childdocumentleveloneuserdefinedrouting"));
                var childDocs2 =
                    context.Search<ChildDocumentLevelOneUserDefinedRouting>(
                        BuildSearchForChildDocumentsWithIdAndParentType(parentDocument2.Id, "parentdocumentuserdefinedrouting"));
                var childDocs3 =
                    context.Search<ChildDocumentLevelTwoUserDefinedRouting>(BuildSearchForChildDocumentsWithIdAndParentType(22,
                        "childdocumentleveloneuserdefinedrouting"));

                Assert.AreEqual(1, childDocs.PayloadResult.Hits.Total);
                Assert.AreEqual(2, childDocs2.PayloadResult.Hits.Total);
                Assert.AreEqual(4, childDocs3.PayloadResult.Hits.Total);

                context.DeleteDocument<ChildDocumentLevelTwoUserDefinedRouting>(73, new RoutingDefinition { ParentId = 22, RoutingId = 7 });
                context.DeleteDocument<ChildDocumentLevelTwoUserDefinedRouting>(72, new RoutingDefinition { ParentId = 22, RoutingId = 7 });
                context.SaveChanges();

                Thread.Sleep(1000);

                childDocs3 =
                    context.Search<ChildDocumentLevelTwoUserDefinedRouting>(BuildSearchForChildDocumentsWithIdAndParentType(22,
                        "childdocumentleveloneuserdefinedrouting"));


                Assert.AreEqual(2, childDocs3.PayloadResult.Hits.Total);

            }
        }

        [Fact]
        public void TestCreateIndexNewChildItemTest()
        {
            const int parentId = 21;
            // This could return NOT FOUND 404 or OK 200. It all depends is the routing matches the same shard. It does not search for the exact parent

            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                var testObject = new ChildDocumentLevelTwoUserDefinedRouting()
                {
                    Id = 46,
                    D3 = "p7.p21.p46"
                };

                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(testObject, testObject.Id, new RoutingDefinition { ParentId = parentId, RoutingId = 7});

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.AreEqual(ret.Status, HttpStatusCode.OK);

                Thread.Sleep(1500);
                var roundTripResult = context.GetDocument<ChildDocumentLevelTwoUserDefinedRouting>(testObject.Id, new RoutingDefinition { ParentId = parentId, RoutingId = 7 });

                var childDocs = context.Search<ChildDocumentLevelTwoUserDefinedRouting>(BuildSearchForChildDocumentsWithIdAndParentType(parentId, "childdocumentleveloneuserdefinedrouting"));
                Assert.IsNotNull(childDocs.PayloadResult.Hits.HitsResult.First(t => t.Id.ToString() == "46"));
                Assert.AreEqual(testObject.Id, roundTripResult.Id);
            }
        }

        [Fact]
        public void TestCreateIndexNewChildItemTestParentDoesNotExist()
        {
            const int parentId = 332;
            // This creates a new child doc with the parent 332 even though no parent for 332 exists
            
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                var testObject = new ChildDocumentLevelTwoUserDefinedRouting
                {
                    Id = 47,
                    D3 = "DoesNotExist.p332.p47"
                };

                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(testObject, testObject.Id, new RoutingDefinition { ParentId = parentId, RoutingId = 7});

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.AreEqual(ret.Status, HttpStatusCode.OK);

                Thread.Sleep(1500);

                var roundTripResult = context.GetDocument<ChildDocumentLevelTwoUserDefinedRouting>(testObject.Id, new RoutingDefinition { ParentId = parentId, RoutingId = 7 });

                var childDocs = context.Search<ChildDocumentLevelTwoUserDefinedRouting>(BuildSearchForChildDocumentsWithIdAndParentType(parentId, "childdocumentleveloneuserdefinedrouting"));
                Assert.IsNotNull(childDocs.PayloadResult.Hits.HitsResult.First(t => t.Id.ToString() == "47"));
                Assert.AreEqual(testObject.Id, roundTripResult.Id);
            }
        }

        [Fact]
        public void TestSearchById()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var childDoc = context.SearchById<ChildDocumentLevelTwoUserDefinedRouting>(71);
                Assert.IsNotNull(childDoc);
                Assert.AreEqual(71, childDoc.Id);
            }
        }

        [Fact]
        public void TestParentSearchById()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var childDoc = context.SearchById<ParentDocumentUserDefinedRouting>(7);
                Assert.IsNotNull(childDoc);
                Assert.AreEqual(7, childDoc.Id);
            }
        }

        [Fact]
        public void TestDocumentExists()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var found = context.DocumentExists<ParentDocumentUserDefinedRouting>(7);
                Assert.IsTrue(found);
            }
        }

        [Fact]
        public void TestDocumentExistsChildDoc()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var found = context.DocumentExists<ChildDocumentLevelTwoUserDefinedRouting>(71, new RoutingDefinition { ParentId = 22, RoutingId = 7 });
                Assert.IsTrue(found);
            }
        }

        [Fact]
        [ExpectedException(typeof(ElasticsearchCrudException))]
        public void TestDocumentExistsChildDocBadRoute()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.DocumentExists<ChildDocumentLevelTwoUserDefinedRouting>(71);
            }
        }

        [Fact]
        public void TestDocumentCountChildDocument()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var found = context.Count<ChildDocumentLevelTwoUserDefinedRouting>();
                Assert.Greater(found,0);
            }
        }
    
        [Fact]
        public void TestDocumentCountChildDocumentWithQuery()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var found = context.Count<ChildDocumentLevelTwoUserDefinedRouting>(BuildSearchForChildDocumentsWithIdAndParentType(22, "childdocumentleveloneuserdefinedrouting"));
                Assert.Greater(found,0);
            }
        }
        
        [Fact]
        [ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextSearch: HttpStatusCode.NotFound")]
        public void TestParentSearchByIdNotFoundWrongType()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var childDoc = context.SearchById<ParentDocumentUserDefinedRouting>(71);
                Assert.IsNotNull(childDoc);
                Assert.AreEqual(7, childDoc.Id);
            }
        }

        [Fact]
        [ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextSearch: HttpStatusCode.NotFound")]
        public void TestParentSearchByIdNotFoundWrongTypeChildDoc()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var childDoc = context.SearchById<ChildDocumentLevelOneUserDefinedRouting>(71);
                Assert.IsNotNull(childDoc);
                Assert.AreEqual(7, childDoc.Id);
            }
        }


        [Fact]
        [ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextSearch: HttpStatusCode.NotFound")]
        public void TestsearchByIdNotFound()
        {
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = new ConsoleTraceProvider();

                var childDoc = context.SearchById<ChildDocumentLevelTwoUserDefinedRouting>(767761);
                Assert.IsNull(childDoc);
            }
        }

        [Fact]
        [ExpectedException(ExpectedException = typeof(ElasticsearchCrudException))]
        public void TestCreateIndexNewChildItemExceptionMissingParentIdTest()
        {
            const int parentId = 21;
            using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                var testObject = new ChildDocumentLevelTwoUserDefinedRouting()
                {
                    Id = 46,
                    D3 = "p7.p21.p46"
                };

                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(testObject, testObject.Id, new RoutingDefinition { ParentId = parentId, RoutingId = 7});

                // Save to Elasticsearch
                var ret = context.SaveChanges();
                Assert.AreEqual(ret.Status, HttpStatusCode.OK);

                var roundTripResult = context.GetDocument<ChildDocumentLevelTwoUserDefinedRouting>(testObject.Id);

                Assert.AreEqual(testObject.Id, roundTripResult.Id);
            }
        }

        private void TestCreateCompletelyNewIndex(ITraceProvider trace)
        {
            var parentDocument = ParentDocument();

            _elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ChildDocumentLevelOneUserDefinedRouting),
                new ElasticsearchMappingChildDocumentForParentUserDefinedRouting());
            _elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ChildDocumentLevelTwoUserDefinedRouting),
                new ElasticsearchMappingChildDocumentForParentUserDefinedRouting());
            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, SaveChildObjectsAsWellAsParent, ProcessChildDocumentsAsSeparateChildIndex, UserDefinedRouting)))
            {
                context.TraceProvider = trace;
                context.AddUpdateDocument(parentDocument, parentDocument.Id);

                // Save to Elasticsearch
                var ret = context.SaveChangesAndInitMappings();
                Assert.AreEqual(ret.Status, HttpStatusCode.OK);

                context.GetDocument<ParentDocumentUserDefinedRouting>(parentDocument.Id);
            }
        }

        private static ParentDocumentUserDefinedRouting ParentDocument()
        {
            var parentDocument = new ParentDocumentUserDefinedRouting
            {
                Id = 7,
                D1 = "p7",
                ChildDocumentLevelOne = new Collection<ChildDocumentLevelOneUserDefinedRouting>
                {
                    new ChildDocumentLevelOneUserDefinedRouting
                    {
                        D2 = "p7.p21",
                        Id = 21,
                        ChildDocumentLevelTwo = new ChildDocumentLevelTwoUserDefinedRouting
                        {
                            Id = 31,
                            D3 = "p7.p21.p31"
                        }
                    },
                    new ChildDocumentLevelOneUserDefinedRouting
                    {
                        D2 = "p7.p22",
                        Id = 22,
                        ChildDocumentLevelTwo = new ChildDocumentLevelTwoUserDefinedRouting
                        {
                            Id = 32,
                            D3 = "p7.p21.p32"
                        },
                        ChildDocumentLevelTwoFromTop = new[]
                        {
                            new ChildDocumentLevelTwoUserDefinedRouting
                            {
                                Id = 71,
                                D3 = "p7.p22.testComplex71"
                            },
                            new ChildDocumentLevelTwoUserDefinedRouting
                            {
                                Id = 72,
                                D3 = "p7.p22.testComplex72"
                            },
                            new ChildDocumentLevelTwoUserDefinedRouting
                            {
                                Id = 73,
                                D3 = "p7.p22.testComplex73"
                            }
                        }
                    }				
                }
            };
            return parentDocument;
        }

        private static ParentDocumentUserDefinedRouting ParentDocument2()
        {
            var parentDocument = new ParentDocumentUserDefinedRouting
            {
                Id = 8,
                D1 = "p8",
                ChildDocumentLevelOne = new Collection<ChildDocumentLevelOneUserDefinedRouting>
                {
                    new ChildDocumentLevelOneUserDefinedRouting
                    {
                        D2 = "p8.p25",
                        Id = 25,
                        ChildDocumentLevelTwo = new ChildDocumentLevelTwoUserDefinedRouting
                        {
                            Id = 35,
                            D3 = "p8.p25.p35"
                        }
                    },
                    new ChildDocumentLevelOneUserDefinedRouting
                    {
                        D2 = "p8.p26",
                        Id = 26,
                        ChildDocumentLevelTwo = new ChildDocumentLevelTwoUserDefinedRouting
                        {
                            Id = 36,
                            D3 = "p8.p26.p36"
                        },
                        ChildDocumentLevelTwoFromTop = new[]
                        {
                            new ChildDocumentLevelTwoUserDefinedRouting
                            {
                                Id = 81,
                                D3 = "p8.p26.testComplex81"
                            }
                        }
                    }
                }
            };
            return parentDocument;
        }
    }

    public class ElasticsearchMappingChildDocumentForParentUserDefinedRouting : ElasticsearchMapping
    {
        public override string GetIndexForType(Type type)
        {
            return "parentdocumentuserdefinedroutings";
        }
    }

    public class ParentDocumentUserDefinedRouting
    {
        [Key]
        public long Id { get; set; }
        public string D1 { get; set; }

        public virtual ICollection<ChildDocumentLevelOneUserDefinedRouting> ChildDocumentLevelOne { get; set; }	
    }

    public class ChildDocumentLevelOneUserDefinedRouting
    {
        [Key]
        public long Id { get; set; }
        public string D2 { get; set; }

        public virtual ChildDocumentLevelTwoUserDefinedRouting[] ChildDocumentLevelTwoFromTop { get; set; }
        public virtual ChildDocumentLevelTwoUserDefinedRouting ChildDocumentLevelTwo { get; set; }
    }

    public class ChildDocumentLevelTwoUserDefinedRouting
    {
        [Key]
        public long Id { get; set; }
        public string D3 { get; set; }
    }
}
