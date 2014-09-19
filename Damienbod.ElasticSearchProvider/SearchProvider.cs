using System;
using System.Collections.Generic;
using System.Linq;
using Damienbod.BusinessLayer.Attributes;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.BusinessLayer.Providers;
using ElasticLinq;

namespace Damienbod.ElasticSearchProvider
{
    [TransientLifetime]
    public class SearchProvider : ISearchProvider
    {
	    readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost:9201/"),null,null,null,"animals");
	    private readonly ElasticContext _context;

        public SearchProvider()
        {
			_context = new ElasticContext(connection);

			//var uri = new Uri("http://localhost:9201");
			//var settings = new ConnectionSettings(uri).SetDefaultIndex("animals");
			//_elasticsearchClient = new ElasticClient(settings);

        }

        public void CreateAnimal(Animal animal)
        {
	        ElasticSearchContext elasticSearchContext = new ElasticSearchContext("http://localhost:9201/");
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
			var tt = _context.Query<Animal>().ToList();
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
	        return _context.Query<Animal>().FirstOrDefault(t => t.Id == id);
        }
    }
}
