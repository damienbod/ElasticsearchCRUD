using System.Collections.Generic;
using System.Linq;
using Damienbod.BusinessLayer.DomainModel;

namespace Damienbod.BusinessLayer.Providers
{
    public interface ISearchProvider
    {
        void CreateAnimal(Animal animal);

        void UpdateAnimal(Animal animal);

		IQueryable<Animal> GetAnimals();
        void DeleteById(int id);
        void DeleteIndex(string index);
        Animal GetAnimal(int id);
    }
}
