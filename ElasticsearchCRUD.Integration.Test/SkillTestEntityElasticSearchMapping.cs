using System;
using ElasticsearchCRUD.Model;
using Newtonsoft.Json;

namespace ElasticsearchCRUD.Integration.Test
{
	public class SkillTestEntityElasticsearchMapping : ElasticsearchMapping
	{
		public override void MapEntityValues(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool beginMappingTree = false, bool createPropertyMappings = false)
		{
			// Map entities to exact name, not lower case
			var propertyInfo = entityInfo.Document.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				MapValue(prop.Name, prop.GetValue(entityInfo.Document), elasticsearchCrudJsonWriter.JsonWriter);
			}
		}

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
			return "coolindex";
		}
	}
}
