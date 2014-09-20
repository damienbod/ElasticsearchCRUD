using System;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using Damienbod.AnimalProvider;
using Damienbod.BusinessLayer.DomainModel;
using Damienbod.BusinessLayer.Managers;

namespace WebApiODataElasticsearch.Controllers
{
	[ODataRoutePrefix("Animal")]
	public class AnimalController : ODataController
	{
		private readonly IAnimalManager _animalManager;

		public AnimalController()
		{
			// TODO add this with DI
			_animalManager = new AnimalManager(new SearchProvider());
		}
		public IHttpActionResult Get()
		{
			return Ok(_animalManager.GetAnimals());
		}

		[ODataRoute()]
		[EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
		public IHttpActionResult Get([FromODataUri] int key)
		{
			return Ok(_animalManager.GetAnimal(key));
		}

		[ODataRoute()]
		[HttpPost]
		[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
		public IHttpActionResult Post(Animal animal)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			_animalManager.CreateAnimal(animal);
			return Created(animal);
		}


		[ODataRoute("({key})")]
		[HttpPut]
		[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
		public IHttpActionResult Put([FromODataUri]int key, [FromBody]Animal animal)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (key != animal.Id)
			{
				return BadRequest();
			}

			_animalManager.UpdateAnimal(animal);

			return Updated(animal);
		}

		[ODataRoute("")]
		[HttpDelete]
		[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
		public IHttpActionResult Delete([FromODataUri] int key)
		{
			_animalManager.DeleteAnimal(key);
			return Content(HttpStatusCode.NoContent, "Deleted");
		}

		[ODataRoute()]
		[HttpPatch]
		[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
		public IHttpActionResult Patch([FromODataUri] int key, Delta<Animal> delta)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var animal = _animalManager.GetAnimal(key);

			delta.Patch(animal);
			_animalManager.UpdateAnimal(animal);
			return Updated(animal);
		}
    }
}
