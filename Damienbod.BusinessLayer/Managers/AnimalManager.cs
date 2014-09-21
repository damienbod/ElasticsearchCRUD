using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using Damienbod.BusinessLayer.Attributes;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.BusinessLayer.Providers;

namespace Damienbod.BusinessLayer.Managers
{
    [TransientLifetime]
    public class AnimalManager : IAnimalManager
    {
        private readonly IElasticsearchCrudProvider _searchProvider;

        public AnimalManager(IElasticsearchCrudProvider searchProvider)
        {
            _searchProvider = searchProvider;
        }

		public Animal GetAnimal(int id)
        {
            return _searchProvider.GetAnimal(id);
        }

        public void UpdateAnimal(Animal value)
        {
            _searchProvider.UpdateAnimal(value);
        }

        public void DeleteAnimal(int id)
        {
            _searchProvider.DeleteById(id);
        }

        public void CreateAnimal(Animal value)
        {
            _searchProvider.CreateAnimal(value);
        }
    }
}
