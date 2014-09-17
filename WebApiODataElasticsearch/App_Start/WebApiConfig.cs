using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Damienbod.BusinessLayer.DomainModel;
using Microsoft.OData.Edm;

namespace WebApiODataElasticsearch
{
	public static class WebApiConfig
	{

		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();

			config.MapODataServiceRoute(
				routeName: "odataRoute",
				routePrefix: "odata",
				model: GetModel());
		
		}

		public static IEdmModel GetModel()
		{
			ODataModelBuilder builder = new ODataConventionModelBuilder();
			builder.Namespace = "D";
			builder.ContainerName = "Default";

			EntitySetConfiguration<Animal> animals = builder.EntitySet<Animal>("Animal");

			return builder.GetEdmModel();
		}
	}
}
