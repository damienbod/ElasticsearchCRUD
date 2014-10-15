using System;
using ElasticsearchCRUD;
using Newtonsoft.Json;

namespace ConsoleElasticsearchCrudExample
{
	public class SkillElasticSearchMapping : ElasticSearchMapping
	{
		/// <summary>
		/// Only required if you have some special mapping or want to remove some properties or use attributes..
		/// </summary>
		/// <param name="entity"></param>
		public override void MapEntityValues(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool beginMappingTree = false)
		{
			Skill skillEntity = entityInfo.Entity as Skill;
			MapValue("id", skillEntity.Id, elasticsearchCrudJsonWriter.JsonWriter);
			MapValue("name", skillEntity.Name, elasticsearchCrudJsonWriter.JsonWriter);
			MapValue("description", skillEntity.Description, elasticsearchCrudJsonWriter.JsonWriter);
			MapValue("created", skillEntity.Created.UtcDateTime, elasticsearchCrudJsonWriter.JsonWriter);
			MapValue("updated", skillEntity.Updated.UtcDateTime, elasticsearchCrudJsonWriter.JsonWriter);
		}

		/// <summary>
		/// Use this if you require special mapping for the elasticsearch document type. For example you could pluralize your Type or set everything to lowercase
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override string GetDocumentType(Type type)
		{
			return "skill";
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