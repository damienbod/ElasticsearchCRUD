using System.Linq;
using System.Threading.Tasks;
using Damienbod.BusinessLayer.DomainModel;

namespace Damienbod.BusinessLayer.Managers
{
    public interface IAnimalManager
    {
        IQueryable<Animal> GetAnimals();
		Task<Animal> GetAnimal(int id);
        void UpdateAnimal(Animal value);
        void DeleteAnimal(int id);
        void CreateAnimal(Animal value);
    }
}
