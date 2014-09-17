using System.Web.Http;
using System.Web.Mvc;

namespace WebApiODataElasticsearch
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}
