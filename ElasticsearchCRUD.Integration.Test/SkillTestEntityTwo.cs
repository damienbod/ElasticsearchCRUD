using System;

namespace ElasticsearchCRUD.Integration.Test
{
	public class SkillTestEntityTwo
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}
}