using System.Linq;
using System.Threading.Tasks;
using Damienbod.BusinessLayer.DomainModel;

namespace Damienbod.BusinessLayer.Providers
{
    public interface ISearchProvider
    {
        void CreateAnimal(Animal animal);
        void UpdateAnimal(Animal animal);
        void DeleteById(int id);
		Task<Animal> GetAnimal(int id);
		IQueryable<Animal> GetAnimals();
    }
}
