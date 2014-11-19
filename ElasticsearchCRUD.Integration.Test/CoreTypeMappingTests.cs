using System;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class CoreTypeMappingTests
	{
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
