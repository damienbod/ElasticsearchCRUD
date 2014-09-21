using Damienbod.BusinessLayer.DomainModel;

namespace Damienbod.BusinessLayer.Providers
{
    public interface IElasticsearchCrudProvider
    {
        void CreateAnimal(Animal animal);
        void UpdateAnimal(Animal animal);
        void DeleteById(int id);
		Animal GetAnimal(int id);
    }
}
