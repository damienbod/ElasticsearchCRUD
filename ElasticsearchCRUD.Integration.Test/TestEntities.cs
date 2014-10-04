using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD.Integration.Test
{
	
	public class SkillTestEntity
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}

	public class SkillTestEntityNoIndex
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}

	public class SkillTestEntityTwo
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}

	public class SkillWithIntArray
	{
		public long Id { get; set; }
		public String BlahBlah { get; set; }
		public List<int> MyIntArray { get; set; }
	}

	public class SkillWithSingleChild
	{
		public long Id { get; set; }
		public String BlahBlah { get; set; }
		public SkillSingleChildElement MySkillSingleChildElement { get; set; }
	}

	public class SkillSingleChildElement
	{
		public long Id { get; set; }
		public String Details { get; set; }
	}
}
