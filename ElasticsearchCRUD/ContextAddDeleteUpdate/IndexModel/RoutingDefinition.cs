using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class RoutingDefinition
	{
		public object RoutingId { get; set; }
		public object ParentId { get; set; }

		public static string GetRoutingUrl(RoutingDefinition routingDefinition)
		{
			var parameters = new ParameterCollection();

			if (routingDefinition != null && routingDefinition.ParentId != null)
			{
				parameters.Add("parent", routingDefinition.ParentId.ToString());
			}
			if (routingDefinition != null && routingDefinition.RoutingId != null)
			{
				parameters.Add("routing", routingDefinition.RoutingId.ToString());
			}

			return parameters.ToString();
		}
	}
}
