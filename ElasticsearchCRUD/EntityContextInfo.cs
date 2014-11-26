using System;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;

namespace ElasticsearchCRUD
{
	public class EntityContextInfo
	{
		public EntityContextInfo()
		{
			RoutingDefinition = new RoutingDefinition();
		}

		public string Id { get; set; }
		public bool DeleteDocument { get; set; }
		public Type EntityType { get; set; }
		public Type ParentEntityType { get; set; }
		public RoutingDefinition RoutingDefinition { get; set; }
		public object Document { get; set; }
	}
}
