using System;
using System.Reflection;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
	public class AnimalElasticSearchMapping : ElasticSearchSerializerMapping<Animal>
	{
		public override void MapEntityValues(Animal entity)
		{
			MapValue("Id", entity.Id);
			MapValue("AnimalType", entity.AnimalType);
			MapValue("TypeSpecificForAnimalType", entity.TypeSpecificForAnimalType);
			MapValue("Description", entity.Description);
			MapValue("Gender", entity.Gender);
			MapValue("LastLocation", entity.LastLocation);
			MapValue("DateOfBirth", entity.DateOfBirth.UtcDateTime);
			MapValue("CreatedTimestamp", entity.CreatedTimestamp.UtcDateTime);
			MapValue("UpdatedTimestamp", entity.UpdatedTimestamp.UtcDateTime);
		}

		//
		// Only required if you have some special logic.
		//
		//public override Animal ParseEntity(JToken source)
		//{
		//	return JsonConvert.DeserializeObject(source.ToString(), typeof(Animal)) as Animal;
		//}


		///// <summary>
		///// Use this if you require special mapping for the elasticsearch document type. For example you could pluralize your Type or set everything to lowercase
		///// </summary>
		///// <param name="type"></param>
		///// <returns></returns>
		//public override string GetDocumentType(Type type)
		//{
		//	return type.Name.ToLower();
		//}
	}

}