using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

/*		----------- EXAMPLE -------------
 *      Only works if the index has already been mapped

POST http:/localhost:9200/parentdocuments/parentdocument/7?_index 
{"id":7,"d1":"p7"}

PUT http://localhost:9200/parentdocuments/childdocumentlevelone/_mapping 
{
  "childdocumentlevelone":{
    "_parent": {"type": "parentdocument"}
  }
}

POST http://localhost:9200/parentdocuments/childdocumentlevelone/21?parent=7
{"id":21,"d2":"p21"}

PUT http://localhost:9200/parentdocuments/childdocumentleveltwo/_mapping
{
  "childdocumentleveltwo":{
    "_parent": {"type": "childdocumentlevelone"}
  }
}

POST http://localhost:9200/parentdocuments/childdocumentleveltwo/31?parent=21 
{"id":31,"d3":"p31"}
 
http://localhost:9200/parentdocuments/parentdocument/_search
{
  "query": {
    "has_child": {
      "type": "childdocumentlevelone",
      "query" : {
        "filtered": {
          "query": { "match_all": {}}
         
        }
      }
    }
  }
}

http://localhost:9200/parentdocuments/childdocumentleveltwo/_search
{
  "query": {
    "has_parent": {
      "type": "childdocumentlevelone",
      "query" : {
        "filtered": {
          "query": { "match_all": {}}
         
        }
      }
    }
  }
}

// Search for a + to n relationship
http://localhost:9200/parentdocuments/childdocumentlevelone/_search
{
  "query": {
    "filtered": {
      "query": {"match_all": {}},
      "filter": {
        "and": [
          {"term": {"id": 21}},
          {
            "has_parent": {
              "type": "parentdocument",
              "query": {
                "term": {"id": "7"}
              }
            }
          }
        ]
      }
    }
  }
}

http://localhost:9200/parentdocuments/22/_search
{
  "query": {
    "filtered": {
      "query": {"match_all": {}},
      "filter": {
        "and": [
          { "term": {"id": 46}},
          { "has_parent": { "type": "childdocumentlevelone", "query": { "term": {"id": "21"} } } }
        ]
      }
    }
  }
}

{
  "query": {
    "filtered": {
      "filter":{
        "has_parent": {
          "type": "childdocumentlevelone",
          "query" : {
            "filtered": {
              "query": { "term": {"id": "21"}}
            }
          }
        }
      }
    }
  } 

{
  "query": {
    "term": {
      "_parent": "childdocumentlevelone#26"
    }
  }
}
 
*/

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class OneToNEntitiesWithChildDocumentsTest
	{
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();


		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			TestCreateCompletelyNewIndex(new ConsoleTraceProvider());
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			//using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			//{
			//	context.AllowDeleteForIndex = true;
			//	var entityResult = context.DeleteIndexAsync<ParentDocument>();
			//	entityResult.Wait();
			//}
		}

		[Test]
		public void TestGetChildItemTestParentDoesNotExist()
		{
			const int parentId = 22;
			// This could return NOT FOUND 404 or OK 200. It all depends is the routing matches the same shard. It does not search for the exact parent

			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				var roundTripResult = context.GetDocument<ChildDocumentLevelTwo>(71, parentId);

				var childDocs = context.Search<ChildDocumentLevelTwo>(BuildSearchForChildDocumentsWithIdAndParentType(parentId, "childdocumentlevelone"));
				Assert.IsNotNull(childDocs.PayloadResult.First(t => t.Id == 71));
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

		[Test]
		public void TestAddUpdateWithExistingMappingsTest()
		{		
			TestAddUpdateWithExistingMappings();
		}

		[Test]
		public void TestCreateIndexNewChildItemTest()
		{
			const int parentId = 21;
			// This could return NOT FOUND 404 or OK 200. It all depends is the routing matches the same shard. It does not search for the exact parent

			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				var testObject = new ChildDocumentLevelTwo
				{
					Id = 46,
					D3 = "p7.p21.p46"
				};

				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(testObject, testObject.Id, parentId);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetDocument<ChildDocumentLevelTwo>(testObject.Id, parentId);

				var childDocs = context.Search<ChildDocumentLevelTwo>(BuildSearchForChildDocumentsWithIdAndParentType(parentId, "childdocumentlevelone"));
				Assert.IsNotNull(childDocs.PayloadResult.First(t => t.Id == 46));
				Assert.AreEqual(testObject.Id, roundTripResult.Id);
			}
		}

		[Test]
		public void TestCreateIndexNewChildItemTestParentDoesNotExist()
		{
			const int parentId = 332;
			// This creates a new child doc with the parent 332 even though no parent for 332 exists
			
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				var testObject = new ChildDocumentLevelTwo
				{
					Id = 47,
					D3 = "DoesNotExist.p332.p47"
				};

				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(testObject, testObject.Id, parentId);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetDocument<ChildDocumentLevelTwo>(testObject.Id, parentId);

				var childDocs = context.Search<ChildDocumentLevelTwo>(BuildSearchForChildDocumentsWithIdAndParentType(parentId, "childdocumentlevelone"));
				Assert.IsNotNull(childDocs.PayloadResult.First(t => t.Id == 47));
				Assert.AreEqual(testObject.Id, roundTripResult.Id);
			}
		}

		[Test]
		public void TestSearchById()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var childDoc = context.SearchById<ChildDocumentLevelTwo>(71);
				Assert.IsNotNull(childDoc);
				Assert.AreEqual(71, childDoc.Id);
			}
		}

		[Test]
		public void TestParentSearchById()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var childDoc = context.SearchById<ParentDocument>(7);
				Assert.IsNotNull(childDoc);
				Assert.AreEqual(7, childDoc.Id);
			}
		}

		[Test]
		public void TestDocumentExists()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var found = context.DocumentExists<ParentDocument>(7);
				Assert.IsTrue(found);
			}
		}

		[Test]
		public void TestDocumentExistsChildDoc()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var found = context.DocumentExists<ChildDocumentLevelTwo>(71, 22);
				Assert.IsTrue(found);
			}
		}

		[Test]
		[ExpectedException(typeof(ElasticsearchCrudException))]
		public void TestDocumentExistsChildDocBadRoute()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var found = context.DocumentExists<ChildDocumentLevelTwo>(71);
			}
		}
		
		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextSearch: HttpStatusCode.NotFound")]
		public void TestParentSearchByIdNotFoundWrongType()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var childDoc = context.SearchById<ParentDocument>(71);
				Assert.IsNotNull(childDoc);
				Assert.AreEqual(7, childDoc.Id);
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextSearch: HttpStatusCode.NotFound")]
		public void TestParentSearchByIdNotFoundWrongTypeChildDoc()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var childDoc = context.SearchById<ChildDocumentLevelOne>(71);
				Assert.IsNotNull(childDoc);
				Assert.AreEqual(7, childDoc.Id);
			}
		}


		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "ElasticsearchContextSearch: HttpStatusCode.NotFound")]
		public void TestsearchByIdNotFound()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var childDoc = context.SearchById<ChildDocumentLevelTwo>(767761);
				Assert.IsNull(childDoc);
			}
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException),ExpectedMessage = "HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...")]
		public void TestCreateIndexNewChildItemExceptionMissingParentIdTest()
		{
			const int parentId = 21;
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				var testObject = new ChildDocumentLevelTwo
				{
					Id = 46,
					D3 = "p7.p21.p46"
				};

				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(testObject, testObject.Id, parentId);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetDocument<ChildDocumentLevelTwo>(testObject.Id);

				Assert.AreEqual(testObject.Id, roundTripResult.Id);
			}
		}

		private void TestCreateCompletelyNewIndex(ITraceProvider trace)
		{
			var parentDocument = ParentDocument();

			_elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof (ChildDocumentLevelOne),
				new ElasticSearchMappingChildDocumentForParent());
			_elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof (ChildDocumentLevelTwo),
				new ElasticSearchMappingChildDocumentForParent());
			using (
				var context = new ElasticSearchContext("http://localhost:9200/",
					new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = trace;
				context.AddUpdateDocument(parentDocument, parentDocument.Id);

				// Save to Elasticsearch
				var ret = context.SaveChangesAndInitMappingsForChildDocuments();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetDocument<ParentDocument>(parentDocument.Id);

				// TODO when implemented
				//var roundTripResultChildDocumentLevelOne =
				//	context.GetDocument<ChildDocumentLevelOne>(parentDocument.ChildDocumentLevelOne.First().Id, parentDocument.Id);
				//Assert.AreEqual(parentDocument.Id, roundTripResult.Id);
				//Assert.AreEqual(parentDocument.ChildDocumentLevelOne.First().Id, roundTripResultChildDocumentLevelOne.Id);
			}
		}

		private void TestAddUpdateWithExistingMappings()
		{
			var parentDocument2 = ParentDocument2();

			using (
				var context = new ElasticSearchContext("http://localhost:9200/",
					new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(parentDocument2, parentDocument2.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetDocument<ParentDocument>(parentDocument2.Id);
				var roundTripResultChildDocumentLevelOne =
					context.GetDocument<ChildDocumentLevelOne>(parentDocument2.ChildDocumentLevelOne.First().Id, parentDocument2.Id);
				var roundTripResultChildDocumentLevelTwo =
					context.GetDocument<ChildDocumentLevelTwo>(parentDocument2.ChildDocumentLevelOne.First().ChildDocumentLevelTwo.Id,
						parentDocument2.ChildDocumentLevelOne.First().Id);
				Assert.AreEqual(parentDocument2.Id, roundTripResult.Id);
				Assert.AreEqual(parentDocument2.ChildDocumentLevelOne.First().Id, roundTripResultChildDocumentLevelOne.Id);
				Assert.AreEqual(parentDocument2.ChildDocumentLevelOne.First().ChildDocumentLevelTwo.Id, roundTripResultChildDocumentLevelTwo.Id);

				var childDocs = context.Search<ChildDocumentLevelTwo>(BuildSearchForChildDocumentsWithIdAndParentType(parentDocument2.ChildDocumentLevelOne.FirstOrDefault().Id, "childdocumentlevelone"));
				var childDocs2 = context.Search<ChildDocumentLevelOne>(BuildSearchForChildDocumentsWithIdAndParentType(parentDocument2.Id, "parentdocument"));
				var childDocs3 = context.Search<ChildDocumentLevelTwo>(BuildSearchForChildDocumentsWithIdAndParentType(22, "childdocumentlevelone"));
			
				Assert.AreEqual(1, childDocs.TotalHits);
				Assert.AreEqual(2, childDocs2.TotalHits);
				Assert.AreEqual(4, childDocs3.TotalHits);
			}
		}

		private static ParentDocument ParentDocument()
		{
			var parentDocument = new ParentDocument
			{
				Id = 7,
				D1 = "p7",
				ChildDocumentLevelOne = new Collection<ChildDocumentLevelOne>
				{
					new ChildDocumentLevelOne
					{
						D2 = "p7.p21",
						Id = 21,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwo
						{
							Id = 31,
							D3 = "p7.p21.p31"
						}
					},
					new ChildDocumentLevelOne
					{
						D2 = "p7.p21.p22",
						Id = 22,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwo
						{
							Id = 32,
							D3 = "p7.p21.p32"
						},
						ChildDocumentLevelTwoFromTop = new ChildDocumentLevelTwo[]
						{
							new ChildDocumentLevelTwo
							{
								Id = 71,
								D3 = "p7.p21.testComplex71"
							},
							new ChildDocumentLevelTwo
							{
								Id = 72,
								D3 = "p7.p21.testComplex72"
							},
							new ChildDocumentLevelTwo
							{
								Id = 73,
								D3 = "p7.p21.testComplex73"
							}
						}
					}				
				}
			};
			return parentDocument;
		}

		private static ParentDocument ParentDocument2()
		{
			var parentDocument = new ParentDocument
			{
				Id = 8,
				D1 = "p8",
				ChildDocumentLevelOne = new Collection<ChildDocumentLevelOne>
				{
					new ChildDocumentLevelOne
					{
						D2 = "p8.p25",
						Id = 25,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwo
						{
							Id = 35,
							D3 = "p8.p25.p35"
						}
					},
					new ChildDocumentLevelOne
					{
						D2 = "p8.p26",
						Id = 26,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwo
						{
							Id = 36,
							D3 = "p8.p26.p36"
						},
						ChildDocumentLevelTwoFromTop = new ChildDocumentLevelTwo[]
						{
							new ChildDocumentLevelTwo
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

	public class ElasticSearchMappingChildDocumentForParent : ElasticSearchMapping
	{
		public override string GetIndexForType(Type type)
		{
			return "parentdocuments";
		}
	}

	public class ParentDocument
	{
		[Key]
		public long Id { get; set; }
		public string D1 { get; set; }

		public virtual ICollection<ChildDocumentLevelOne> ChildDocumentLevelOne { get; set; }	
	}

	public class ChildDocumentLevelOne
	{
		[Key]
		public long Id { get; set; }
		public string D2 { get; set; }

		public virtual ChildDocumentLevelTwo[] ChildDocumentLevelTwoFromTop { get; set; }
		public virtual ChildDocumentLevelTwo ChildDocumentLevelTwo { get; set; }
	}

	public class ChildDocumentLevelTwo
	{
		[Key]
		public long Id { get; set; }
		public string D3 { get; set; }
	}
}
