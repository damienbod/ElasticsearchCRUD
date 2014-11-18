using System;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class CoreTypeMappingTests
	{
		[Test]
		public void ReflectionAttributeElasticsearchString()
		{
			var testSkillParentObject = new MappingTypeTestEntity
			{
				CreatedSkillParent = DateTime.UtcNow,
				UpdatedSkillParent = DateTime.UtcNow,
				DescriptionSkillParent = "A test entity description",
				Id = 7,
				NameSkillParent = "cool",
				SkillChildren = new[] { new SkillChild{} }
			};

			var propertyInfo = testSkillParentObject.GetType().GetProperties();
			foreach (var property in propertyInfo)
			{
				if (Attribute.IsDefined(property, typeof(ElasticsearchString)))
				{
					var obj = property.Name.ToLower(); // (testSkillParentObject);
					object[] attrs = property.GetCustomAttributes(typeof(ElasticsearchCoreTypes), true);

					Console.WriteLine(obj + " : " + (attrs[0] as ElasticsearchCoreTypes).JsonString());
				}
			}
		}
	}

	public class MappingTypeTestEntity
	{
		public long Id { get; set; }
		[ElasticsearchString(Boost = 1.0)]
		public string NameSkillParent { get; set; }

		[ElasticsearchString(Boost = 1.4, Index = StringIndex.analyzed)]
		public string DescriptionSkillParent { get; set; }

		public DateTimeOffset CreatedSkillParent { get; set; }
		public DateTimeOffset UpdatedSkillParent { get; set; }

		public virtual SkillChild[] SkillChildren { get; set; }
	}
}
