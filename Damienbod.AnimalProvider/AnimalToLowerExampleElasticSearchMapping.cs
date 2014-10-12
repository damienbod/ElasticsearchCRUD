using System;
using ElasticsearchCRUD;
using Newtonsoft.Json;

namespace Damienbod.AnimalProvider
{
	public class AnimalToLowerExampleElasticSearchMapping : ElasticSearchMapping
	{
		/// <summary>
		/// Here you can do any type of entity mapping
		/// </summary>
		public override void MapEntityValues(Object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool beginMappingTree = false)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				MapValue(prop.Name, prop.GetValue(entity), elasticsearchCrudJsonWriter.JsonWriter);
			}
		}

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