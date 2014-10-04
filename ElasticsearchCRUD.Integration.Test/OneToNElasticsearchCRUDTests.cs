using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Damienbod.BusinessLayer.DomainModel;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{

	[TestFixture]
	public class OneToNElasticsearchCRUDTests
	{
		private List<SkillParent> _entitiesForSkillParent;
		private List<SkillChild> _entitiesForSkillChild;
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

		[SetUp]
		public void SetUp()
		{
			_entitiesForSkillParent = new List<SkillParent>();
			_entitiesForSkillChild = new List<SkillChild>();
			// Create a 100 entities

			var entity1 = new SkillParent
			{

				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 1,
				NameSkillParent = "cool"
			};

			var entity2 = new SkillParent
			{

				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 2,
				NameSkillParent = "cool"
			};

			for (int i = 0; i < 3; i++)
			{
				var entityTwo = new SkillChild
				{
					CreatedSkillChild = DateTime.UtcNow,
					UpdatedSkillChild = DateTime.UtcNow,
					DescriptionSkillChild = "A test SkillChild description",
					Id = i,
					NameSkillChild = "cool"
				};

				_entitiesForSkillChild.Add(entityTwo);
			}

			// a parent with one child
			entity1.SkillChildren = new List<SkillChild>();
			entity1.SkillChildren.Add(_entitiesForSkillChild[0]);
			_entitiesForSkillParent.Add(entity1);

			// a parent with two children
			entity2.SkillChildren = new List<SkillChild>();
			entity2.SkillChildren.Add(_entitiesForSkillChild[1]);
			entity2.SkillChildren.Add(_entitiesForSkillChild[2]);
			_entitiesForSkillParent.Add(entity2);
		}

		[TearDown]
		public void TearDown()
		{
			//_entitiesForTests = null;
			// SkillTestEntity SkillWithIntArray SkillWithSingleChild SkillSingleChildElement
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult1 = context.DeleteIndexAsync<SkillTestEntity>();
				entityResult1.Wait();
				var entityResult2 = context.DeleteIndexAsync<SkillTestEntityTwo>();
				entityResult2.Wait();
				var entityResult3 = context.DeleteIndexAsync<SkillParent>();
				entityResult3.Wait();
				var entityResult4 = context.DeleteIndexAsync<SkillChild>();
				entityResult4.Wait();
				var entityResult5 = context.DeleteIndexAsync<SkillTestEntity>();
				entityResult5.Wait();
				//var entityResult6 = context.DeleteIndexAsync<SkillWithIntArray>();
				//.Wait();
				var entityResult7 = context.DeleteIndexAsync<SkillWithSingleChild>();
				entityResult7.Wait();
				var entityResult8 = context.DeleteIndexAsync<SkillSingleChildElement>();
				entityResult8.Wait();
				var entityResult9 = context.DeleteIndexAsync<Animal>();
				entityResult9.Wait();
			}
		}

		[Test]
		public void TestDefaultContextParentWithTwoChildren()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{

				context.AddUpdateEntity(_entitiesForSkillParent[1], 1);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		public void TestDefaultContextParentNestedIntCollection()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithIntArray = new SkillWithIntCollection
				{
					MyIntArray = new List<int> {2, 4, 6, 99, 7},
					BlahBlah = "test3 with int array",
					Id = 2
				};
				context.AddUpdateEntity(skillWithIntArray, skillWithIntArray.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var returned = context.GetEntity<SkillWithIntCollection>(2);
				Assert.AreEqual(skillWithIntArray.MyIntArray[2], returned.MyIntArray[2]);
			}
		}

		[Test]
		public void TestDefaultContextParentNestedIntArray()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithIntArray = new SkillWithIntArray
				{
					MyIntArray = new int[] { 2, 4, 6, 99, 7 },
					BlahBlah = "test3 with int array",
					Id = 2
				};
				context.AddUpdateEntity(skillWithIntArray, skillWithIntArray.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var returned = context.GetEntity<SkillWithIntArray>(2);
				Assert.AreEqual(skillWithIntArray.MyIntArray[2], returned.MyIntArray[2]);
			}
		}

		[Test]
		public void TestDefaultContextParentNestedIntCollectionEqualsNull()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithIntArray = new SkillWithIntCollection {BlahBlah = "test with no int array", Id = 3};

				context.AddUpdateEntity(skillWithIntArray, skillWithIntArray.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);


				var returned = context.GetEntity<SkillWithIntCollection>(3);
				Assert.AreEqual(skillWithIntArray.BlahBlah, returned.BlahBlah, "Round Trip not the same");
				Assert.IsNull(returned.MyIntArray);
			}
		}


		[Test]
		public void TestDefaultContextParentNestedIntArrayEqualsNull()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithIntArray = new SkillWithIntArray { BlahBlah = "test with no int array", Id = 3 };
				context.AddUpdateEntity(skillWithIntArray, skillWithIntArray.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var returned = context.GetEntity<SkillWithIntArray>(3);
				Assert.AreEqual(skillWithIntArray.BlahBlah, returned.BlahBlah, "Round Trip not the same");
				Assert.IsNull(returned.MyIntArray);
			}
		}

		[Test]
		public void TestDefaultContextParentNestedSkillWithStringAndLongCollection()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithIntArray = new SkillWithStringLongAndDoubleCollection
				{
					MyStringArray = new List<String>
					{
						"one", "two","three"
					},
					BlahBlah = "test3 with int array",
					Id = 2,
					MyDoubleArray = new List<double> {2.4, 5.7, 67.345 },
					MyLongArray = new List<long> {34444445, 65432, 7889999 }
				};
				context.AddUpdateEntity(skillWithIntArray, skillWithIntArray.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var returned = context.GetEntity<SkillWithStringLongAndDoubleCollection>(2);
				Assert.AreEqual(skillWithIntArray.MyStringArray[2], returned.MyStringArray[2]);
				Assert.AreEqual(skillWithIntArray.MyDoubleArray[1], returned.MyDoubleArray[1]);
				Assert.AreEqual(skillWithIntArray.MyLongArray[1], returned.MyLongArray[1]);
			}
		}

		[Test]
		public void TestDefaultContextParentNestedSkillWithStringAndLongArray()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithIntArray = new SkillWithStringLongAndDoubleArray
				{
					MyStringArray = new string[]
					{
						"one", "two","three"
					},
					BlahBlah = "test3 with int array",
					Id = 2,
					MyDoubleArray = new double[] { 2.4, 5.7, 67.345 },
					MyLongArray = new long[] { 34444445, 65432, 7889999 }
				};
				context.AddUpdateEntity(skillWithIntArray, skillWithIntArray.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var returned = context.GetEntity<SkillWithStringLongAndDoubleArray>(2);
				Assert.AreEqual(skillWithIntArray.MyStringArray[2], returned.MyStringArray[2]);
				Assert.AreEqual(skillWithIntArray.MyDoubleArray[1], returned.MyDoubleArray[1]);
				Assert.AreEqual(skillWithIntArray.MyLongArray[1], returned.MyLongArray[1]);
			}
		}
		

		[Test]
		public void TestDefaultContextParentWithSingleChild()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithSingleChild = new SkillWithSingleChild
				{
					Id = 4,
					BlahBlah = "TEST",
					MySkillSingleChildElement = new SkillSingleChildElement{ Id= 4, Details = "tteesstt"}
				};
				context.AddUpdateEntity(skillWithSingleChild, skillWithSingleChild.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}
		}

		[Test]
		public void TestDefaultContextParentWithSingleChildEqualsNull()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skillWithSingleChild = new SkillWithSingleChild {MySkillSingleChildElement = null};
				context.AddUpdateEntity(skillWithSingleChild, skillWithSingleChild.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);
			}
		}
	}
}
