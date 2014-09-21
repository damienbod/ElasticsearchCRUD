using System;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
	public class AnimalToLowerExampleElasticSearchMapping : ElasticSearchSerializerMapping<Animal>
	{
		/// <summary>
		/// Here you can do any type of entity mapping
		/// </summary>
		/// <param name="entity"></param>
		public override void MapEntityValues(Animal entity)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				MapValue(prop.Name.ToLower(), prop.GetValue(entity));
			}
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