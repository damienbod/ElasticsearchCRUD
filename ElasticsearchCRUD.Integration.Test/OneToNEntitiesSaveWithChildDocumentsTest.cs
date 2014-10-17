using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
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
*/

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class OneToNEntitiesWithChildDocumentsTest
	{

		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<ParentDocument>();
				entityResult.Wait();
			}
		}

		[Test]
		public void TestDefaultContextParentWithACollectionOfOneChildDocuments()
		{
			var parentDocument = ParentDocument();

			_elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ChildDocumentLevelOne), new ElasticSearchMappingChildDocumentForParent());
			_elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ChildDocumentLevelTwo), new ElasticSearchMappingChildDocumentForParent());
			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true,true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateEntity(parentDocument, parentDocument.Id);

				// Save to Elasticsearch
				var ret = context.SaveChangesAndInitMappingsForChildDocuments();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<ParentDocument>(parentDocument.Id);
				var roundTripResultChildDocumentLevelOne = context.GetEntity<ChildDocumentLevelOne>(parentDocument.ChildDocumentLevelOne.First().Id, parentDocument.Id);
				Assert.AreEqual(parentDocument.Id, roundTripResult.Id);
				Assert.AreEqual(parentDocument.ChildDocumentLevelOne.First().Id, roundTripResultChildDocumentLevelOne.Id);
			}

			var parentDocument2 = ParentDocument2();

			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateEntity(parentDocument2, parentDocument2.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<ParentDocument>(parentDocument2.Id);
				var roundTripResultChildDocumentLevelOne = context.GetEntity<ChildDocumentLevelOne>(parentDocument2.ChildDocumentLevelOne.First().Id, parentDocument2.Id);
				var roundTripResultChildDocumentLevelTwo = context.GetEntity<ChildDocumentLevelTwo>(parentDocument2.ChildDocumentLevelOne.First().ChildDocumentLevelTwo.Id, parentDocument2.ChildDocumentLevelOne.First().Id);
				Assert.AreEqual(parentDocument2.Id, roundTripResult.Id);
				Assert.AreEqual(parentDocument2.ChildDocumentLevelOne.First().Id, roundTripResultChildDocumentLevelOne.Id);
				Assert.AreEqual(parentDocument2.ChildDocumentLevelOne.First().ChildDocumentLevelTwo.Id, roundTripResultChildDocumentLevelTwo.Id);
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
						}
					}				
				},
				ChildDocumentLevelTwoFromTop = new ChildDocumentLevelTwo[]
				{
					new ChildDocumentLevelTwo
					{
						Id = 71,
						D3 = "p7.testComplex71"
					},
					new ChildDocumentLevelTwo
					{
						Id = 72,
						D3 = "p7.testComplex72"
					},
					new ChildDocumentLevelTwo
					{
						Id = 73,
						D3 = "p7.testComplex73"
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
						}
					}
				},
				ChildDocumentLevelTwoFromTop = new ChildDocumentLevelTwo[]
				{
					new ChildDocumentLevelTwo
					{
						Id = 81,
						D3 = "p8.testComplex81"
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
		public virtual ChildDocumentLevelTwo[] ChildDocumentLevelTwoFromTop { get; set; }
	}

	public class ChildDocumentLevelOne
	{
		[Key]
		public long Id { get; set; }
		public string D2 { get; set; }

		public virtual ChildDocumentLevelTwo ChildDocumentLevelTwo { get; set; }
	}

	public class ChildDocumentLevelTwo
	{
		[Key]
		public long Id { get; set; }
		public string D3 { get; set; }
	}
}
