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
			//var elasticSearchContext = new ElasticSearchContext<Skill>("http://localhost:9201/", "skill", new SkillElasticSearchMapping());
			var elasticSearchContext = new ElasticSearchContext<Skill>("http://localhost:9201/", "skill");
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

			// Add 2 new entities
			elasticSearchContext.AddUpdateEntity(skillEf, skillEf.Id.ToString(CultureInfo.InvariantCulture));
			elasticSearchContext.AddUpdateEntity(skillOrm, skillOrm.Id.ToString(CultureInfo.InvariantCulture));
			 var ret = elasticSearchContext.SaveChangesAsync();

			Console.ReadLine();

			// get a entity and update it, then delete an entity
			Skill singleEntityWithId = elasticSearchContext.GetEntity("2").Result.PayloadResult;
			singleEntityWithId.Updated = DateTime.UtcNow;
			elasticSearchContext.AddUpdateEntity(skillOrm, skillOrm.Id.ToString(CultureInfo.InvariantCulture));
			elasticSearchContext.DeleteEntity("1");
			ret = elasticSearchContext.SaveChangesAsync();

			elasticSearchContext.AddUpdateEntity(skillEf, skillEf.Id.ToString(CultureInfo.InvariantCulture));
			ret = elasticSearchContext.SaveChangesAsync();
		}
	}
}
