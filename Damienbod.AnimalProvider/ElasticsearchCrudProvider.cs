using Damienbod.BusinessLayer.DomainModel;
using ElasticsearchCRUD;

namespace Damienbod.AnimalProvider
{
	public class ElasticsearchCrudProvider 
    {
		readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();
		private readonly ElasticSearchContext _elasticSearchContext;
		public ElasticsearchCrudProvider()
		{
			// You don't need to create this Resolver everytime a new context is created...
			_elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Animal), new AnimalToLowerExampleElasticSearchMapping());
			_elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(new ElasticSearchMappingResolver(), true,false));
		}

        public void CreateAnimal(Animal animal)
        {
			_elasticSearchContext.AddUpdateDocument(animal, animal.Id);
			var ret = _elasticSearchContext.SaveChangesAsync();    
        }

        public void UpdateAnimal(Animal animal)
        {
			_elasticSearchContext.AddUpdateDocument(animal, animal.Id);
			var ret = _elasticSearchContext.SaveChangesAsync(); 
        }

        public void DeleteById(int id)
        {
			_elasticSearchContext.DeleteDocument<Animal>(id);
			var ret = _elasticSearchContext.SaveChangesAsync(); 
        }

        public Animal GetAnimal(int id)
        {
	        return  _elasticSearchContext.GetDocumentAsync<Animal>(id).Result.PayloadResult;
        }
    }
}
