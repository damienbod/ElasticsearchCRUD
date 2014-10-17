using System;

namespace ElasticsearchCRUD
{
	public class EntityContextInfo
	{
		public string Id { get; set; }
		public bool DeleteEntity { get; set; }
		public Type EntityType { get; set; }
		public Type ParentEntityType { get; set; }
		public object ParentId { get; set; }
		public object Entity { get; set; }
	}
}
