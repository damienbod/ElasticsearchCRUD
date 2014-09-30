using System;

namespace ConsoleElasticsearchCrudExample
{
	public static class TestData
	{
		public static Skill SkillEf = new Skill
		{
			Created = DateTime.UtcNow,
			Updated = DateTime.UtcNow,
			Description = "C# Entity Framework V6, .NET 4.5",
			Id = 11,
			Name = "C# Entity Framework"
		};

		public static Skill SkillOrm = new Skill
		{
			Created = DateTime.UtcNow,
			Updated = DateTime.UtcNow,
			Description = "ORM frameworks in .NET",
			Id = 12,
			Name = "ORM"
		};

		public static Skill SkillSQLServer = new Skill
		{
			Created = DateTime.UtcNow,
			Updated = DateTime.UtcNow,
			Description = "MS SQL Server 2014",
			Id = 13,
			Name = "MS SQL Server 2014"
		};

		public static Skill SkillGermanWithFunnyLetters = new Skill
		{
			Created = DateTime.UtcNow,
			Updated = DateTime.UtcNow,
			Description = "Deutsch öäü",
			Id = 14,
			Name = "German languages skills"
		};

		public static SkillLevel SkillLevel = new SkillLevel()
		{
			Created = DateTime.UtcNow,
			Updated = DateTime.UtcNow,
			Description = "expert",
			Id = 1,
		};
	}
}
