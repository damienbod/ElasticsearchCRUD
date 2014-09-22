using System.Globalization;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
	public class ElasticsearchCrudProvider 
    {
		readonly ElasticSearchContext<Animal> _elasticSearchContext = new ElasticSearchContext<Animal>("http://localhost:9200/", Animal.SearchIndex, new AnimalToLowerExampleElasticSearchMapping());

        public void CreateAnimal(Animal animal)
        {
			_elasticSearchContext.AddUpdateEntity(animal, animal.Id.ToString(CultureInfo.InvariantCulture));
			var ret = _elasticSearchContext.SaveChangesAsync();    
        }

        public void UpdateAnimal(Animal animal)
        {
			_elasticSearchContext.AddUpdateEntity(animal, animal.Id.ToString(CultureInfo.InvariantCulture));
			var ret = _elasticSearchContext.SaveChangesAsync(); 
        }

        public void DeleteById(int id)
        {
			_elasticSearchContext.DeleteEntity(id.ToString(CultureInfo.InvariantCulture));
			var ret = _elasticSearchContext.SaveChangesAsync(); 
        }

        public Animal GetAnimal(int id)
        {
	        return  _elasticSearchContext.GetEntity(id.ToString(CultureInfo.InvariantCulture)).Result.PayloadResult;
        }
    }
}
