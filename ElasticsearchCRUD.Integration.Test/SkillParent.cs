using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD.Integration.Test
{
	public class SkillParent
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }
		public string DescriptionSkillParent { get; set; }
		public DateTimeOffset CreatedSkillParent { get; set; }
		public DateTimeOffset UpdatedSkillParent { get; set; }

		public virtual ICollection<SkillChild> SkillChildren { get; set; }
	}

	public class SkillChild
	{
		public long Id { get; set; }
		public string NameSkillChild { get; set; }
		public string DescriptionSkillChild { get; set; }
		public DateTimeOffset CreatedSkillChild { get; set; }
		public DateTimeOffset UpdatedSkillChild { get; set; }
	}
}