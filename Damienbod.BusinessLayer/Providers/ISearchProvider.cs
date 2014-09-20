using System.Linq;
using Damienbod.BusinessLayer.DomainModel;

namespace Damienbod.BusinessLayer.Providers
{
    public interface ISearchProvider
    {
        void CreateAnimal(Animal animal);
        void UpdateAnimal(Animal animal);
        void DeleteById(int id);
		Animal GetAnimal(int id);
		IQueryable<Animal> GetAnimals();
    }
}
