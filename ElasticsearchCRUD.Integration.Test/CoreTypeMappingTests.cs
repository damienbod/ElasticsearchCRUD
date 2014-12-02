using System;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class CoreTypeMappingTests
	{
		[Test]
		public void OptimizeParametersPefaultTest()
		{
			var optimizeParameters = new OptimizeParameters();
			Assert.IsNullOrEmpty(optimizeParameters.GetOptimizeParameters());
		}

		[Test]
		public void OptimizeParametersPefaultSetAll()
		{
			var optimizeParameters = new OptimizeParameters
			{
				Flush = false,
				NumberOfShards = 4,
				OnlyExpungeDeletesSet = true,
				WaitForMerge = false
			};

			Assert.AreEqual(optimizeParameters.GetOptimizeParameters(), "?max_num_segments=4&only_expunge_deletes=true&flush=false&wait_for_merge=false");
		}

		[Test]
		public void ReflectionAttributeElasticsearchMappings()
		{
			var testSkillParentObject = new MappingTypeTestEntity
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 7,
				NameSkillParent = "cool",
			};

			var propertyInfo = testSkillParentObject.GetType().GetProperties();
			foreach (var property in propertyInfo)
			{
				if (Attribute.IsDefined(property, typeof(ElasticsearchCoreTypes)))
				{
					var obj = property.Name.ToLower(); 
					object[] attrs = property.GetCustomAttributes(typeof(ElasticsearchCoreTypes), true);

					Console.WriteLine(obj + " : " + (attrs[0] as ElasticsearchCoreTypes).JsonString());
				}
			}
		}

		[Test]
		public void JsonDeserializeObjectSearchResultNullArrayOfHits()
		{

			const string responseString = "{\"took\":3,\"timed_out\":false,\"_shards\":{\"total\":5,\"successful\":5,\"failed\":0},\"hits\":{\"total\":0,\"max_score\":null,\"hits\":[]}}";

			var responseObject = JObject.Parse(responseString);

			//JsonConvert.DeserializeObject(responseString, typeof(SearchResult<string>));
			SearchResult<ParentDocument> result = responseObject.ToObject<SearchResult<ParentDocument>>();

		}

		[Test]
		public void JsonDeserializeObjectSearchResultNullSource()
		{

			const string responseString = "{\"took\":4,\"timed_out\":false,\"_shards\":{\"total\":3,\"successful\":3,\"failed\":0},\"hits\":{\"total\":1,\"max_score\":1.0,\"hits\":[{\"_index\":\"mappingtypesourcetests\",\"_type\":\"mappingtypesourcetest\",\"_id\":\"1\",\"_score\":1.0}]}}";

			var responseObject = JObject.Parse(responseString);

			JsonConvert.DeserializeObject(responseString, typeof(SearchResult<string>));
			SearchResult<ParentDocument> result = responseObject.ToObject<SearchResult<ParentDocument>>();

		}
	}

	public class FieldDataDefinition
	{
		[ElasticsearchString(Index=StringIndex.not_analyzed)]
		public string Raw { get; set; }
	}

	public class MappingTypeTestEntity
	{
		[ElasticsearchLong(DocValues = false)]
		public long Id { get; set; }
		[ElasticsearchString(Boost = 1.0)]
		public string NameSkillParent { get; set; }

		[ElasticsearchDouble(Boost = 2.3, Index = NumberIndex.no)]
		public double DoubleTestId { get; set; }

		[ElasticsearchFloat(Boost = 2.2, Index = NumberIndex.no)]
		public float FloatTestId { get; set; }

		[ElasticsearchShort(Boost = 2.1, Index = NumberIndex.no)]
		public short ShortTestId { get; set; }

		[ElasticsearchInteger(Boost = 2.5, Index = NumberIndex.no, DocValues = true)]
		public int IntTestId { get; set; }

		[ElasticsearchByte(Store=true, IncludeInAll=true)]
		public byte ByteTestId { get; set; }

		[ElasticsearchString(Boost = 1.7, Index = StringIndex.analyzed, NormsEnabled = true, Fields = typeof(FieldDataDefinition) )]
		public string DescriptionSkillParent { get; set; }

		[ElasticsearchDate(Boost = 1.1)]
		public DateTimeOffset CreatedSkillParent { get; set; }

		public DateTimeOffset UpdatedSkillParent { get; set; }

		[ElasticsearchBoolean(CopyTo = "descriptionskillparent")]
		public bool TestBool { get; set; }

		[ElasticsearchString(CopyToList = new[] { "descriptionskillparent", "nameskillparent"}, NormsEnabled = true, NormsLoading = NormsLoading.eager)]
		public string TestString { get; set; }
	}
}
