using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{

	[TestFixture]
	public class OneToNElasticsearchCRUDTests
	{
		private List<SkillChild> _entitiesForSkillChild;
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

		[SetUp]
		public void SetUp()
		{
			_entitiesForSkillChild = new List<SkillChild>();

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
		}


		[TearDown]
		public void TearDown()
		{
			//_entitiesForTests = null;
			// SkillTestEntity SkillWithIntArray SkillWithSingleChild SkillSingleChildElement
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				//context.DeleteIndexAsync<SkillWithStringAndLongArray>();
				var entityResult2 = context.DeleteIndexAsync<SkillTestEntityTwo>();
				//var entityResult3 = context.DeleteIndexAsync<SkillParent>();
				var entityResult4 = context.DeleteIndexAsync<SkillChild>();
				var entityResult5 = context.DeleteIndexAsync<SkillTestEntity>();
				var entityResult6 = context.DeleteIndexAsync<SkillWithIntArray>();
				var entityResult7 = context.DeleteIndexAsync<SkillWithSingleChild>();
				var entityResult8 = context.DeleteIndexAsync<SkillSingleChildElement>();
				var entityResult9 = context.DeleteIndexAsync<SkillWithIntCollection>();
				var entityResult10 = context.DeleteIndexAsync<SkillWithStringLongAndDoubleArray>();
				var entityResult11 = context.DeleteIndexAsync<SkillWithStringLongAndDoubleCollection>();
				Task.WaitAll();
			}
		}

		[Test]
		public void TestDefaultContextParentWithACollectionOfOneChildObjectNested()
		{
			var testSkillParentObject = new SkillParent
			{

				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 7,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> {_entitiesForSkillChild[0]}
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{

				context.AddUpdateEntity(testSkillParentObject, testSkillParentObject.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<SkillParent>(testSkillParentObject.Id);
				Assert.AreEqual(roundTripResult.DescriptionSkillParent, testSkillParentObject.DescriptionSkillParent);
				Assert.AreEqual(roundTripResult.SkillChildren.First().DescriptionSkillChild, testSkillParentObject.SkillChildren.First().DescriptionSkillChild);
			}
		}

		[Test]
		public void TestDefaultContextParentWithACollectionOfThreeChildObjectsNested()
		{
			var testSkillParentObject = new SkillParent
			{

				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 8,
				NameSkillParent = "cool",
				SkillChildren = new Collection<SkillChild> { _entitiesForSkillChild[0], _entitiesForSkillChild[1], _entitiesForSkillChild[2] }
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{

				context.AddUpdateEntity(testSkillParentObject, testSkillParentObject.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<SkillParent>(testSkillParentObject.Id);
				Assert.AreEqual(roundTripResult.DescriptionSkillParent, testSkillParentObject.DescriptionSkillParent);
				Assert.AreEqual(roundTripResult.SkillChildren.First().DescriptionSkillChild, testSkillParentObject.SkillChildren.First().DescriptionSkillChild);
				Assert.AreEqual(roundTripResult.SkillChildren.ToList()[1].DescriptionSkillChild, testSkillParentObject.SkillChildren.ToList()[1].DescriptionSkillChild);
				Assert.AreEqual(roundTripResult.SkillChildren.ToList()[1].DescriptionSkillChild, testSkillParentObject.SkillChildren.ToList()[1].DescriptionSkillChild);
			}
		}

		[Test]
		public void TestDefaultContextParentWithACollectionOfNoChildObjectNested()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skill = new SkillParent();
				skill.CreatedSkillParent = DateTime.UtcNow;
				skill.UpdatedSkillParent = DateTime.UtcNow;
				skill.Id = 34;
				skill.NameSkillParent = "rr";
				skill.DescriptionSkillParent = "ee";
				skill.SkillChildren = new Collection<SkillChild>();
				context.AddUpdateEntity(skill, skill.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<SkillParent>(skill.Id);
				Assert.AreEqual(roundTripResult.DescriptionSkillParent, skill.DescriptionSkillParent);
			}
		}

		[Test]
		public void TestDefaultContextParentWithNullCollectionNested()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var skill = new SkillParent();
				skill.CreatedSkillParent = DateTime.UtcNow;
				skill.UpdatedSkillParent = DateTime.UtcNow;
				skill.Id = 34;
				skill.NameSkillParent = "rr";
				skill.DescriptionSkillParent = "ee";
				context.AddUpdateEntity(skill, skill.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<SkillParent>(skill.Id);
				Assert.AreEqual(roundTripResult.DescriptionSkillParent, skill.DescriptionSkillParent);
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
					Id = 5,
					BlahBlah = "TEST",
					MySkillSingleChildElement = new SkillSingleChildElement{ Id= 5, Details = "tteesstt"}
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
