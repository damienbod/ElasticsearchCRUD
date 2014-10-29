using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class DocumentsWithChildDocumentsTestNotAllowed
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();

		[Test]
		[ExpectedException(ExpectedException = typeof(ElasticsearchCrudException), ExpectedMessage = "InitMappings: Not supported, child documents can only have one parent")]
		public void TestCreateCompletelyNewIndexExpectException()
		{
			var parentDocument = GetParentDocumentEx();

			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ChildDocumentLevelOneEx),
				new ElasticsearchMappingChildDocumentForParentEx());
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(ChildDocumentLevelTwoEx),
				new ElasticsearchMappingChildDocumentForParentEx());

			using (var context = new ElasticsearchContext("http://localhost:9200/",new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver, true, true)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateDocument(parentDocument, parentDocument.Id);

				// Save to Elasticsearch
				context.SaveChangesAndInitMappingsForChildDocuments();
			}
		}

		private static ParentDocumentEx GetParentDocumentEx()
		{
			var parentDocument = new ParentDocumentEx
			{
				Id = 7,
				D1 = "p7",
				ChildDocumentLevelOne = new Collection<ChildDocumentLevelOneEx>
				{
					new ChildDocumentLevelOneEx
					{
						D2 = "p7.p21",
						Id = 21,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwoEx
						{
							Id = 31,
							D3 = "p7.p21.p31"
						}
					},
					new ChildDocumentLevelOneEx
					{
						D2 = "p7.p21.p22",
						Id = 22,
						ChildDocumentLevelTwo = new ChildDocumentLevelTwoEx
						{
							Id = 32,
							D3 = "p7.p21.p32"
						}
					}
				},
				ChildDocumentLevelTwoFromTop = new[]
				{
					new ChildDocumentLevelTwoEx
					{
						Id = 71,
						D3 = "p7.p21.testComplex71"
					},
					new ChildDocumentLevelTwoEx
					{
						Id = 72,
						D3 = "p7.p21.testComplex72"
					},
					new ChildDocumentLevelTwoEx
					{
						Id = 73,
						D3 = "p7.p21.testComplex73"
					}
				}
			};
			return parentDocument;
		}

		public class ElasticsearchMappingChildDocumentForParentEx : ElasticsearchMapping
		{
			public override string GetIndexForType(Type type)
			{
				return "parentdocumentsex";
			}
		}

		public class ParentDocumentEx
		{
			[Key]
			public long Id { get; set; }
			public string D1 { get; set; }

			public virtual ICollection<ChildDocumentLevelOneEx> ChildDocumentLevelOne { get; set; }

			public virtual ChildDocumentLevelTwoEx[] ChildDocumentLevelTwoFromTop { get; set; }
		}

		public class ChildDocumentLevelOneEx
		{
			[Key]
			public long Id { get; set; }
			public string D2 { get; set; }

			public virtual ChildDocumentLevelTwoEx ChildDocumentLevelTwo { get; set; }
		}

		public class ChildDocumentLevelTwoEx
		{
			[Key]
			public long Id { get; set; }
			public string D3 { get; set; }
		}

	}
}
