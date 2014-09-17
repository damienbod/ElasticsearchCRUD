using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Damienbod.BusinessLayer.Attributes;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.BusinessLayer.Providers;
using Nest;

namespace Damienbod.ElasticSearchProvider
{
    [TransientLifetime]
    public class SearchProvider : ISearchProvider
    {
        private readonly ElasticClient _elasticsearchClient;

        public SearchProvider()
        {
			//var uri = new Uri("http://localhost:9200");
			//var settings = new ConnectionSettings(uri).SetDefaultIndex("animals");
			//_elasticsearchClient = new ElasticClient(settings);

        }

        public void CreateAnimal(Animal animal)
        {
			//ValidateIfIdIsAlreadyUsedForIndex(animal.Id.ToString(CultureInfo.InvariantCulture));               
			//_elasticsearchClient.Index(animal, Animal.SearchIndex, "animal");          
        }

        private void ValidateIfIdIsAlreadyUsedForIndex(string id)
        {
            var idsList = new List<string> { id};
            var result = _elasticsearchClient.Search<Animal>(s => s
                .Index("animals")
                .AllTypes()
                .Query(p => p.Ids(idsList)));
            if (result.Documents.Any()) throw new ArgumentException("Id already exists in store");
        }

        public void UpdateAnimal(Animal animal)
        {
            //_elasticsearchClient.Index(animal, Animal.SearchIndex, "animal");
        }

		public IQueryable<Animal> GetAnimals()
        {
			//var result = _elasticsearchClient.Search<Animal>(s => s
			//		.Index("animals")
			//		.AllTypes()
			//		.MatchAll().Size(250)
			//	);
			//return result.Documents.ToList();
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
			//var idsList = new List<string> { id.ToString(CultureInfo.InvariantCulture) };
			//var result = _elasticsearchClient.Search<Animal>(s => s
			//	.Index("animals")
			//	.AllTypes()
			//	.Query(p => p.Ids(idsList)));

			//return result.Documents.First();
	        return new Animal();
        }
    }
}
