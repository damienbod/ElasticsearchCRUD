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
