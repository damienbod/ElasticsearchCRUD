using System.Web.Http;
using Damienbod.AnimalProvider;
using Damienbod.BusinessLayer.DomainModel;

namespace WebApiElasticsearchCrudExample.Controllers
{
	public class AnimalController : ApiController
	{
		private readonly ElasticsearchCrudProvider _elasticsearchCrudProvider = new ElasticsearchCrudProvider();
		// GET api/Animal/5
		[HttpGet]
		public Animal Get(int id)
		{
			return _elasticsearchCrudProvider.GetAnimal(id);
		}

		// POST api/Animal
		[HttpPost]
		public void Post([FromBody]Animal value)
		{
			_elasticsearchCrudProvider.CreateAnimal(value);
		}

		// PUT api/Animal/5
		[HttpPut]
		public void Put(int id, [FromBody]Animal value)
		{
			_elasticsearchCrudProvider.UpdateAnimal(value);
		}

		// DELETE api/Animal/5
		[HttpDelete]
		public void Delete(int id)
		{
			_elasticsearchCrudProvider.DeleteById(id);
		}
	}
}
