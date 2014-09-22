using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damienbod.ElasticSearchProvider;

namespace ConsoleElasticsearchCrudExample
{
	class Program
	{
		static void Main(string[] args)
		{
			//var elasticSearchContext = new ElasticSearchContext<Skill>("http://localhost:9200/", "skill", new SkillElasticSearchMapping());
			var elasticSearchContext = new ElasticSearchContext<Skill>("http://localhost:9200/", "skill");
			elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
			var skillEf = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "C# Entity Framework V6, .NET 4.5",
				Id = 1,
				Name = "C# Entity Framework"
			};

			var skillOrm = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "ORM frameworks in .NET",
				Id = 2,
				Name = "ORM"
			};

			var skillSQLServer = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "MS SQL Server 2014",
				Id = 3,
				Name = "MS SQL Server 2014"
			};

			var skillGermanWithFunnyLetters = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "Deutsch öäü",
				Id = 4,
				Name = "German languages skills"
			};

			// Add 2 new entities
			elasticSearchContext.AddUpdateEntity(skillEf, skillEf.Id);
			elasticSearchContext.AddUpdateEntity(skillOrm, skillOrm.Id);
			elasticSearchContext.AddUpdateEntity(skillSQLServer, skillSQLServer.Id);
			elasticSearchContext.AddUpdateEntity(skillGermanWithFunnyLetters, skillGermanWithFunnyLetters.Id);

			var ret =  elasticSearchContext.SaveChangesAsync();

			Console.WriteLine(ret.Result.PayloadResult);
			Console.WriteLine(ret.Result.Status);
			Console.WriteLine(ret.Result.Description);
			

			// get a entity and update it, then delete an entity
			Skill singleEntityWithId = elasticSearchContext.GetEntity("4").Result.PayloadResult;
			singleEntityWithId.Updated = DateTime.UtcNow;
			elasticSearchContext.AddUpdateEntity(skillOrm, skillOrm.Id);
			elasticSearchContext.DeleteEntity("1");
			ret = elasticSearchContext.SaveChangesAsync();



			elasticSearchContext.AddUpdateEntity(skillEf, skillEf.Id);
			ret = elasticSearchContext.SaveChangesAsync();
			Console.WriteLine(ret.Result.PayloadResult);
			Console.WriteLine(ret.Result.Status);
			Console.WriteLine(ret.Result.Description);
			Console.ReadLine();
		}
	}
}
