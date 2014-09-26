using System;
using ElasticsearchCRUD;

namespace ConsoleElasticsearchCrudExample
{
	public class SkillElasticSearchMapping : ElasticSearchMapping
	{
		public override void MapEntityValues(object entity)
		{
			Skill skillEntity = entity as Skill;
			MapValue("id", skillEntity.Id);
			MapValue("name", skillEntity.Name);
			MapValue("description", skillEntity.Description);
			MapValue("created", skillEntity.Created.UtcDateTime);
			MapValue("updated", skillEntity.Updated.UtcDateTime);
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
			return "cooltype";
		}

		/// <summary>
		/// Use this if the index is named differently to the default type.Name.ToLower
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override string GetIndexForType(Type type)
		{
			return "skills";
		}
	}
}