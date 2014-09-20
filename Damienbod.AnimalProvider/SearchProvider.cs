using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Damienbod.BusinessLayer.Attributes;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.BusinessLayer.Providers;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
    [TransientLifetime]
	public class SearchProvider : ISearchProvider
    {
	    readonly ElasticSearchContext<Animal> _elasticSearchContext = new ElasticSearchContext<Animal>("http://localhost:9201/", Animal.SearchIndex, new AnimalElasticSearchSerializerMapping());

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

		public IQueryable<Animal> GetAnimals()
		{
			return null;
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
