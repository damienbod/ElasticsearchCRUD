using System.Collections.Generic;
using System.Linq;
using Damienbod.BusinessLayer.Attributes;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.BusinessLayer.Providers;
using Damienbod.ElasticSearchProvider;

namespace Damienbod.AnimalProvider
{
    [TransientLifetime]
    public class SearchProvider : ISearchProvider
    {

        public SearchProvider()
        {

        }

        public void CreateAnimal(Animal animal)
        {
	        var elasticSearchContext = new ElasticSearchContext<Animal>("http://localhost:9201/", new AnimalElasticSearchSerializerMapping());
	        var ret = elasticSearchContext.SendEntitiesAsync(new List<Animal> {animal}, Animal.SearchIndex);

	        //ValidateIfIdIsAlreadyUsedForIndex(animal.Id.ToString(CultureInfo.InvariantCulture));               
	        //_elasticsearchClient.Index(animal, Animal.SearchIndex, "animal");          
        }

        private void ValidateIfIdIsAlreadyUsedForIndex(string id)
        {
			//var idsList = new List<string> { id};
			//var result = _elasticsearchClient.Search<Animal>(s => s
			//	.Index("animals")
			//	.AllTypes()
			//	.Query(p => p.Ids(idsList)));
			//if (result.Documents.Any()) throw new ArgumentException("Id already exists in store");
        }

        public void UpdateAnimal(Animal animal)
        {
            //_elasticsearchClient.Index(animal, Animal.SearchIndex, "animal");
        }

		public IQueryable<Animal> GetAnimals()
		{
			return null;
		}

        public void DeleteById(int id)
        {
           // _elasticsearchClient.DeleteById("animals", "animal", id);
        }

        public void DeleteIndex(string index)
        {
            //_elasticsearchClient.DeleteIndex(index);
        }

        public Animal GetAnimal(int id)
        {
	        return null;
        }
    }
}
