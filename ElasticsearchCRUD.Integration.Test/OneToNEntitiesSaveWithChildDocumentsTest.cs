using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class OneToNEntitiesWithChildDocumentsTest
	{
/*		------------------------
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
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

		[SetUp]
		public void SetUp()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		[Test]
		public void TestDefaultContextParentWithACollectionOfOneChildDocuments()
		{
			var parentDocument = new ParentDocument
			{
				Id = 7,
				D1 = "p7",
				ChildDocumentLevelOne = new Collection<ChildDocumentLevelOne>
				{
					new ChildDocumentLevelOne
					{
						D2 = "p21",
						Id = 21,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwo
						{
							Id=31,
							D3="p31"
						}
					},
					
					new ChildDocumentLevelOne
					{
						D2 = "p22",
						Id = 22,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwo
						{
							Id=32,
							D3="p32"
						}
					}
				}
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(_elasticSearchMappingResolver, true,true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateEntity(parentDocument, parentDocument.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<ParentDocument>(parentDocument.Id);
				Assert.AreEqual(parentDocument.Id, roundTripResult.Id);
			}
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

		public virtual ChildDocumentLevelTwo ChildDocumentLevelTwo { get; set; }
	}

	public class ChildDocumentLevelTwo
	{
		[Key]
		public long Id { get; set; }
		public string D3 { get; set; }
	}
}
