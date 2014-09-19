using Damienbod.BusinessLayer.DomainModel;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
	public class AnimalElasticSearchSerializerMapping : ElasticSearchSerializerMapping<Animal>
	{
		public override void WriteJsonEntry(Animal entity)
		{
			WriteValue("Id", entity.Id);
			WriteValue("AnimalType", entity.AnimalType);
			WriteValue("TypeSpecificForAnimalType", entity.TypeSpecificForAnimalType);
			WriteValue("Description", entity.Description);
			WriteValue("Gender", entity.Gender);
			WriteValue("LastLocation", entity.LastLocation);
			WriteValue("DateOfBirth", entity.DateOfBirth.UtcDateTime);
			WriteValue("CreatedTimestamp", entity.CreatedTimestamp.UtcDateTime);
			WriteValue("UpdatedTimestamp", entity.UpdatedTimestamp.UtcDateTime);
		}
	}

}