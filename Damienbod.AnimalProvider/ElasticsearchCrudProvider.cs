using Damienbod.BusinessLayer.DomainModel;
using ElasticsearchCRUD;

namespace Damienbod.AnimalProvider
{
	public class ElasticsearchCrudProvider 
    {
		readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private readonly ElasticsearchContext _elasticsearchContext;
		public ElasticsearchCrudProvider()
		{
			// You don't need to create this Resolver everytime a new context is created...
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Animal), new AnimalToLowerExampleElasticsearchMapping());
			_elasticsearchContext = new ElasticsearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(new ElasticsearchMappingResolver(), true,false));
		}

        public void CreateAnimal(Animal animal)
        {
			_elasticsearchContext.AddUpdateDocument(animal, animal.Id);
			var ret = _elasticsearchContext.SaveChangesAsync();    
        }

        public void UpdateAnimal(Animal animal)
        {
			_elasticsearchContext.AddUpdateDocument(animal, animal.Id);
			var ret = _elasticsearchContext.SaveChangesAsync(); 
        }

        public void DeleteById(int id)
        {
			_elasticsearchContext.DeleteDocument<Animal>(id);
			var ret = _elasticsearchContext.SaveChangesAsync(); 
        }

        public Animal GetAnimal(int id)
        {
	        return  _elasticsearchContext.GetDocumentAsync<Animal>(id).Result.PayloadResult;
        }
    }
}
