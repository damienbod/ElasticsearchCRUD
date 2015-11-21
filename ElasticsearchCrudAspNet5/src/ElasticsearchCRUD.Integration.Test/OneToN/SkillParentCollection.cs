using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD.Integration.Test.OneToN
{
	public class SkillParentCollection
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }
		public string DescriptionSkillParent { get; set; }
		public DateTimeOffset CreatedSkillParent { get; set; }
		public DateTimeOffset UpdatedSkillParent { get; set; }

		public virtual ICollection<SkillChild> SkillChildren { get; set; }
	}

	public class SkillParentArray
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }
		public string DescriptionSkillParent { get; set; }
		public DateTimeOffset CreatedSkillParent { get; set; }
		public DateTimeOffset UpdatedSkillParent { get; set; }

		public virtual SkillChild[] SkillChildren { get; set; }
	}

	public class SkillChild
	{
		public long Id { get; set; }
		public string NameSkillChild { get; set; }
		public string DescriptionSkillChild { get; set; }
		public DateTimeOffset CreatedSkillChild { get; set; }
		public DateTimeOffset UpdatedSkillChild { get; set; }
	}

	public class SkillDocument
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }

		public virtual ICollection<SkillNestedDocumentLevelOne> SkillNestedDocumentLevelOne { get; set; }
	}

	public class SkillNestedDocumentLevelOne
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }

		public virtual ICollection<SkillNestedDocumentLevelTwo> SkillNestedDocumentLevelTwo { get; set; }
	}

	public class SkillNestedDocumentLevelTwo
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }
	}

	public class SkillDocumentHastSet
	{
		public long Id { get; set; }
		public string NameSkillParent { get; set; }

		public virtual HashSet<SkillNestedDocumentLevelTwo> SkillNestedDocumentLevelTwoHashSet { get; set; }
	}
}
