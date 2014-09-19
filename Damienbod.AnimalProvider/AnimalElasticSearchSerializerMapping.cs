using Damienbod.BusinessLayer.DomainModel;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
	public class AnimalElasticSearchSerializerMapping : ElasticSearchSerializerMapping<Animal>
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
	}

}