using System;
using Damienbod.ElasticSearchProvider;

namespace ConsoleElasticsearchCrudExample
{
	public class SkillElasticSearchMapping : ElasticSearchSerializerMapping<Skill>
	{
		public override void MapEntityValues(Skill entity)
		{
			MapValue("id", entity.Id);
			MapValue("name", entity.Name);
			MapValue("description", entity.Description);
			MapValue("created", entity.Created.UtcDateTime);
			MapValue("updated", entity.Updated.UtcDateTime);
		}

		//
		// Only required if you have some special logic.
		//
		//public override Animal ParseEntity(JToken source)
		//{
		//	return JsonConvert.DeserializeObject(source.ToString(), typeof(Animal)) as Animal;
		//}

		/// <summary>
		/// Use this if you require special mapping for the elasticsearch document type. For example you could pluralize your Type or set everything to lowercase
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override string GetDocumentType(Type type)
		{
			return type.Name.ToLower();
		}
	}
}