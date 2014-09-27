using System;
using ElasticsearchCRUD;

namespace Damienbod.AnimalProvider
{
	public class AnimalToLowerExampleElasticSearchMapping : ElasticSearchMapping
	{
		/// <summary>
		/// Here you can do any type of entity mapping
		/// </summary>
		/// <param name="entity"></param>
		public override void MapEntityValues(Object entity)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				MapValue(prop.Name.ToLower(), prop.GetValue(entity));
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