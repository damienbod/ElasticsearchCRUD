using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class ComplexRelationsTests
	{
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

		[TearDown]
		public void TearDown()
		{
			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;
				var entityResult = context.DeleteIndexAsync<TestNestedDocumentLevelOneHashSet>(); entityResult.Wait();
				var secondDelete = context.DeleteIndexAsync<TestNestedDocumentLevelOneCollection>(); secondDelete.Wait();
				var thirdDelete = context.DeleteIndexAsync<TestNestedDocumentLevelOneArray>(); thirdDelete.Wait();
			}
		}

		[Test]
		public void Test1_to_n_1_m_Array()
		{
			var data = new TestNestedDocumentLevelOneArray
			{
				DescriptionOne = "D1",
				Id = 1,
				TestNestedDocumentLevelTwoArray = new TestNestedDocumentLevelTwoArray[]
					{
						new TestNestedDocumentLevelTwoArray
						{
							DescriptionTwo = "D2", 
							Id=1,
							TestNestedDocumentLevelOneArray = new TestNestedDocumentLevelOneArray{
								Id=1,
								DescriptionOne="D1", 
								TestNestedDocumentLevelTwoArray = new TestNestedDocumentLevelTwoArray[]
								{
									new TestNestedDocumentLevelTwoArray
									{
										DescriptionTwo="D1", 
										Id=1
									}
								}
							},
							TestNestedDocumentLevelThreeArray = new TestNestedDocumentLevelThreeArray
							{
								DescriptionThree = "D3", 
								Id=1, 
								TestNestedDocumentLevelFourArray = new TestNestedDocumentLevelFourArray[]
								{
									new TestNestedDocumentLevelFourArray
									{
										DescriptionFour="D4", Id=1, 
										TestNestedDocumentLevelThreeArray = new TestNestedDocumentLevelThreeArray
										{
											DescriptionThree="D3"
										}
									},
									new TestNestedDocumentLevelFourArray
									{
										DescriptionFour="D4", Id=2, 
										TestNestedDocumentLevelThreeArray = new TestNestedDocumentLevelThreeArray
										{
											DescriptionThree="D3"
										}
									}
								}
							}
						}
					}
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.AddUpdateEntity(data, data.Id);
				context.SaveChanges();
			}

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var roundTripData = context.GetEntity<TestNestedDocumentLevelOneArray>(data.Id);
				Assert.AreEqual(data.DescriptionOne, roundTripData.DescriptionOne);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoArray.First().DescriptionTwo,
					roundTripData.TestNestedDocumentLevelTwoArray.First().DescriptionTwo
					);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoArray.First().TestNestedDocumentLevelThreeArray.DescriptionThree,
					roundTripData.TestNestedDocumentLevelTwoArray.First().TestNestedDocumentLevelThreeArray.DescriptionThree
					);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoArray.First().TestNestedDocumentLevelThreeArray.TestNestedDocumentLevelFourArray.First().DescriptionFour,
					roundTripData.TestNestedDocumentLevelTwoArray.First().TestNestedDocumentLevelThreeArray.TestNestedDocumentLevelFourArray.First().DescriptionFour
					);
			}
		}

		[Test]
		public void Test1_to_n_1_m_Collection()
		{
			var data = new TestNestedDocumentLevelOneCollection
			{
				DescriptionOne = "D1",
				Id = 1,
				TestNestedDocumentLevelTwoCollection = new Collection<TestNestedDocumentLevelTwoCollection>
					{
						new TestNestedDocumentLevelTwoCollection
						{
							DescriptionTwo = "D2", 
							Id=1,
							TestNestedDocumentLevelOneCollection = new TestNestedDocumentLevelOneCollection{
								Id=1,
								DescriptionOne="D1", 
								TestNestedDocumentLevelTwoCollection = new Collection<TestNestedDocumentLevelTwoCollection>
								{
									new TestNestedDocumentLevelTwoCollection
									{
										DescriptionTwo="D1", 
										Id=1
									}
								}
							},
							TestNestedDocumentLevelThreeCollection = new TestNestedDocumentLevelThreeCollection
							{
								DescriptionThree = "D3", 
								Id=1, 
								TestNestedDocumentLevelFourCollection = new Collection<TestNestedDocumentLevelFourCollection>
								{
									new TestNestedDocumentLevelFourCollection
									{
										DescriptionFour="D4", Id=1, 
										TestNestedDocumentLevelThreeCollection = new TestNestedDocumentLevelThreeCollection
										{
											DescriptionThree="D3"
										}
									},
									new TestNestedDocumentLevelFourCollection
									{
										DescriptionFour="D4", Id=2, 
										TestNestedDocumentLevelThreeCollection = new TestNestedDocumentLevelThreeCollection
										{
											DescriptionThree="D3"
										}
									}
								}
							}
						}
					}
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(data, data.Id);
				context.SaveChanges();
			}

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var roundTripData = context.GetEntity<TestNestedDocumentLevelOneCollection>(data.Id);
				Assert.AreEqual(data.DescriptionOne, roundTripData.DescriptionOne);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoCollection.First().DescriptionTwo,
					roundTripData.TestNestedDocumentLevelTwoCollection.First().DescriptionTwo
					);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoCollection.First().TestNestedDocumentLevelThreeCollection.DescriptionThree,
					roundTripData.TestNestedDocumentLevelTwoCollection.First().TestNestedDocumentLevelThreeCollection.DescriptionThree
					);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoCollection.First().TestNestedDocumentLevelThreeCollection.TestNestedDocumentLevelFourCollection.First().DescriptionFour,
					roundTripData.TestNestedDocumentLevelTwoCollection.First().TestNestedDocumentLevelThreeCollection.TestNestedDocumentLevelFourCollection.First().DescriptionFour
					);
			}
		}

		[Test]
		public void Test1_to_n_1_m_HashSet()
		{
			var data = new TestNestedDocumentLevelOneHashSet
			{
				DescriptionOne = "D1",
				Id = 1,
				TestNestedDocumentLevelTwoHashSet = new HashSet<TestNestedDocumentLevelTwoHashSet>
					{
						new TestNestedDocumentLevelTwoHashSet
						{
							DescriptionTwo = "D2", 
							Id=1,
							TestNestedDocumentLevelOneHashSet = new TestNestedDocumentLevelOneHashSet{
								Id=1,
								DescriptionOne="D1", 
								TestNestedDocumentLevelTwoHashSet = new HashSet<TestNestedDocumentLevelTwoHashSet>
								{
									new TestNestedDocumentLevelTwoHashSet
									{
										DescriptionTwo="D1", 
										Id=1
									}
								}
							},
							TestNestedDocumentLevelThreeHashSet = new TestNestedDocumentLevelThreeHashSet
							{
								DescriptionThree = "D3", 
								Id=1, 
								TestNestedDocumentLevelFour = new HashSet<TestNestedDocumentLevelFourHashSet>
								{
									new TestNestedDocumentLevelFourHashSet
									{
										DescriptionFour="D4", Id=1, 
										TestNestedDocumentLevelThreeHashSet = new TestNestedDocumentLevelThreeHashSet
										{
											DescriptionThree="D3"
										}
									},
									new TestNestedDocumentLevelFourHashSet
									{
										DescriptionFour="D4", Id=2, 
										TestNestedDocumentLevelThreeHashSet = new TestNestedDocumentLevelThreeHashSet
										{
											DescriptionThree="D3", 
											TestNestedDocumentLevelFour = new HashSet<TestNestedDocumentLevelFourHashSet>
											{
												new TestNestedDocumentLevelFourHashSet{DescriptionFour="D4",
													Id=2
												}
											}
										}
									}
								}
							}
						}
					}
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(data,data.Id);
				context.SaveChanges();
			}

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{
				var roundTripData = context.GetEntity<TestNestedDocumentLevelOneHashSet>(data.Id);
				Assert.AreEqual(data.DescriptionOne, roundTripData.DescriptionOne);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoHashSet.First().DescriptionTwo, 
					roundTripData.TestNestedDocumentLevelTwoHashSet.First().DescriptionTwo
					);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoHashSet.First().TestNestedDocumentLevelThreeHashSet.DescriptionThree,
					roundTripData.TestNestedDocumentLevelTwoHashSet.First().TestNestedDocumentLevelThreeHashSet.DescriptionThree
					);
				Assert.AreEqual(
					data.TestNestedDocumentLevelTwoHashSet.First().TestNestedDocumentLevelThreeHashSet.TestNestedDocumentLevelFour.First().DescriptionFour,
					roundTripData.TestNestedDocumentLevelTwoHashSet.First().TestNestedDocumentLevelThreeHashSet.TestNestedDocumentLevelFour.First().DescriptionFour
					);
			}
		}

		[Test]
		public void TestDefaultContextParentWith2LevelsOfNestedObjects()
		{
			var testSkillParentObject = new SkillDocument
			{
				Id = 7,
				NameSkillParent = "cool",
				SkillNestedDocumentLevelOne = new Collection<SkillNestedDocumentLevelOne>()
				{
					new SkillNestedDocumentLevelOne()
					{
						Id=1,
						NameSkillParent="rr", 
						SkillNestedDocumentLevelTwo= new Collection<SkillNestedDocumentLevelTwo>
						{
							new SkillNestedDocumentLevelTwo
							{
								Id=3, 
								NameSkillParent="eee"
							}
						}
					}
				}
			};

			using (var context = new ElasticSearchContext("http://localhost:9200/", _elasticSearchMappingResolver))
			{

				context.AddUpdateEntity(testSkillParentObject, testSkillParentObject.Id);

				// Save to Elasticsearch
				var ret = context.SaveChanges();
				Assert.AreEqual(ret.Status, HttpStatusCode.OK);

				var roundTripResult = context.GetEntity<SkillDocument>(testSkillParentObject.Id);
				Assert.AreEqual(roundTripResult.NameSkillParent, testSkillParentObject.NameSkillParent);
				Assert.AreEqual(roundTripResult.SkillNestedDocumentLevelOne.First().NameSkillParent, testSkillParentObject.SkillNestedDocumentLevelOne.First().NameSkillParent);
				Assert.AreEqual(
					roundTripResult.SkillNestedDocumentLevelOne.First().SkillNestedDocumentLevelTwo.First().NameSkillParent,
					testSkillParentObject.SkillNestedDocumentLevelOne.First().SkillNestedDocumentLevelTwo.First().NameSkillParent);
			}
		}

	}

	#region test classes HastSet

	public class TestNestedDocumentLevelOneHashSet
	{
		public long Id { get; set; }
		public string DescriptionOne { get; set; }

		public virtual HashSet<TestNestedDocumentLevelTwoHashSet> TestNestedDocumentLevelTwoHashSet { get; set; }
	}

	public class TestNestedDocumentLevelTwoHashSet
	{
		public long Id { get; set; }
		public string DescriptionTwo { get; set; }

		public virtual TestNestedDocumentLevelOneHashSet TestNestedDocumentLevelOneHashSet { get; set; }
		public virtual TestNestedDocumentLevelThreeHashSet TestNestedDocumentLevelThreeHashSet { get; set; }
	}

	public class TestNestedDocumentLevelThreeHashSet
	{
		public long Id { get; set; }
		public string DescriptionThree { get; set; }

		public virtual HashSet<TestNestedDocumentLevelTwoHashSet> TestNestedDocumentLevelTwo { get; set; }

		public virtual HashSet<TestNestedDocumentLevelFourHashSet> TestNestedDocumentLevelFour { get; set; }
	}

	public class TestNestedDocumentLevelFourHashSet
	{
		public long Id { get; set; }
		public string DescriptionFour { get; set; }

		public virtual TestNestedDocumentLevelThreeHashSet TestNestedDocumentLevelThreeHashSet { get; set; }
	}

	#endregion test classes HastSet

	#region test classes Collection

	public class TestNestedDocumentLevelOneCollection
	{
		public long Id { get; set; }
		public string DescriptionOne { get; set; }

		public virtual Collection<TestNestedDocumentLevelTwoCollection> TestNestedDocumentLevelTwoCollection { get; set; }
	}

	public class TestNestedDocumentLevelTwoCollection
	{
		public long Id { get; set; }
		public string DescriptionTwo { get; set; }

		public virtual TestNestedDocumentLevelOneCollection TestNestedDocumentLevelOneCollection { get; set; }
		public virtual TestNestedDocumentLevelThreeCollection TestNestedDocumentLevelThreeCollection { get; set; }
	}

	public class TestNestedDocumentLevelThreeCollection
	{
		public long Id { get; set; }
		public string DescriptionThree { get; set; }

		public virtual List<TestNestedDocumentLevelTwoCollection> TestNestedDocumentLevelTwoCollection { get; set; }

		public virtual Collection<TestNestedDocumentLevelFourCollection> TestNestedDocumentLevelFourCollection { get; set; }
	}

	public class TestNestedDocumentLevelFourCollection
	{
		public long Id { get; set; }
		public string DescriptionFour { get; set; }

		public virtual TestNestedDocumentLevelThreeCollection TestNestedDocumentLevelThreeCollection { get; set; }
	}

	#endregion test classes Collection

	#region test classes Object Array

	public class TestNestedDocumentLevelOneArray
	{
		public long Id { get; set; }
		public string DescriptionOne { get; set; }

		public virtual TestNestedDocumentLevelTwoArray[] TestNestedDocumentLevelTwoArray { get; set; }
	}

	public class TestNestedDocumentLevelTwoArray
	{
		public long Id { get; set; }
		public string DescriptionTwo { get; set; }

		public virtual TestNestedDocumentLevelOneArray TestNestedDocumentLevelOneArray { get; set; }
		public virtual TestNestedDocumentLevelThreeArray TestNestedDocumentLevelThreeArray { get; set; }
	}

	public class TestNestedDocumentLevelThreeArray
	{
		public long Id { get; set; }
		public string DescriptionThree { get; set; }

		public virtual TestNestedDocumentLevelTwoArray[] TestNestedDocumentLevelTwoArray { get; set; }

		public virtual TestNestedDocumentLevelFourArray[] TestNestedDocumentLevelFourArray { get; set; }
	}

	public class TestNestedDocumentLevelFourArray
	{
		public long Id { get; set; }
		public string DescriptionFour { get; set; }

		public virtual TestNestedDocumentLevelThreeArray TestNestedDocumentLevelThreeArray { get; set; }
	}

	#endregion test classes Object Array
}
