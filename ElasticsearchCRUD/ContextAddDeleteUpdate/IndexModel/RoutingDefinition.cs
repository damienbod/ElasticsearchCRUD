namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class RoutingDefinition
	{
		public object RoutingId { get; set; }
		public object ParentId { get; set; }

		public static string GetRoutingUrl(RoutingDefinition routingDefinition)
		{
			string routingUrl = "";

			if (routingDefinition != null && routingDefinition.ParentId != null && routingDefinition.RoutingId != null)
			{
				routingUrl = "?parent=" + routingDefinition.ParentId + "&routing=" + routingDefinition.RoutingId; ;
			}
			else if (routingDefinition != null && routingDefinition.ParentId != null)
			{
				routingUrl = "?parent=" + routingDefinition.ParentId;
			}
			else if (routingDefinition != null && routingDefinition.RoutingId != null)
			{
				routingUrl = "?routing=" + routingDefinition.RoutingId;
			}

			return routingUrl;
		}


	}
}
