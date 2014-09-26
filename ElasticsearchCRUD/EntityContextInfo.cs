using System;

namespace ElasticsearchCRUD
{
	public class EntityContextInfo
	{
		public string Id { get; set; }

		public bool DeleteEntity { get; set; }

		public Type EntityType { get; set; }


	}
}
