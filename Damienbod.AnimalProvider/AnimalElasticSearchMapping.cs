using Damienbod.BusinessLayer.DomainModel;
using Damienbod.ElasticSearchProvider;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

		public override Animal ParseEntity(JToken source)
		{
			return JsonConvert.DeserializeObject(source.ToString(), typeof(Animal)) as Animal;
		}
	}

}