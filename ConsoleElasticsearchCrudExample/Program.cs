using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticsearchCRUD;

namespace ConsoleElasticsearchCrudExample
{
	class Program
	{
		static void Main(string[] args)
		{
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();

			// You only require a mapping if the default settings are not good enough
			//elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Skill), new SkillElasticSearchMapping());
			var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver);

			elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
			var skillEf = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "C# Entity Framework V6, .NET 4.5",
				Id = 11,
				Name = "C# Entity Framework"
			};

			var skillOrm = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "ORM frameworks in .NET",
				Id = 12,
				Name = "ORM"
			};

			var skillSQLServer = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "MS SQL Server 2014",
				Id = 13,
				Name = "MS SQL Server 2014"
			};

			var skillGermanWithFunnyLetters = new Skill
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "Deutsch öäü",
				Id = 14,
				Name = "German languages skills"
			};

			var skillLevel = new SkillLevel()
			{
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow,
				Description = "expert",
				Id = 1,
			};

			// Add 2 new entities
			elasticSearchContext.AddUpdateEntity(skillEf, skillEf.Id);
			elasticSearchContext.AddUpdateEntity(skillOrm, skillOrm.Id);
			elasticSearchContext.AddUpdateEntity(skillSQLServer, skillSQLServer.Id);
			elasticSearchContext.AddUpdateEntity(skillGermanWithFunnyLetters, skillGermanWithFunnyLetters.Id);
			elasticSearchContext.AddUpdateEntity(skillLevel, skillLevel.Id);

			var ret =  elasticSearchContext.SaveChangesAsync();

			Console.WriteLine(ret.Result.PayloadResult);
			Console.WriteLine(ret.Result.Status);
			Console.WriteLine(ret.Result.Description);
			
			// get a entity and update it, then delete an entity
			Skill singleEntityWithId = elasticSearchContext.GetEntity<Skill>("14").Result.PayloadResult;
			singleEntityWithId.Updated = DateTime.UtcNow;
			elasticSearchContext.AddUpdateEntity(skillOrm, skillOrm.Id);
			elasticSearchContext.DeleteEntity<Skill>("11");
			ret = elasticSearchContext.SaveChangesAsync();

			elasticSearchContext.AddUpdateEntity(skillEf, skillEf.Id);
			ret = elasticSearchContext.SaveChangesAsync();
			Console.WriteLine(ret.Result.PayloadResult);
			Console.WriteLine(ret.Result.Status);
			Console.WriteLine(ret.Result.Description);
			Console.ReadLine();

			// deleting indexes are usually not required...
			//elasticSearchContext.AllowDeleteForIndex = true;
			//var result = elasticSearchContext.DeleteIndex<SkillLevel>();
			//Console.WriteLine(result.Result.PayloadResult);
			//Console.WriteLine(result.Result.Status);
			//Console.WriteLine(result.Result.Description);
			//Console.ReadLine();
		}
	}
}
