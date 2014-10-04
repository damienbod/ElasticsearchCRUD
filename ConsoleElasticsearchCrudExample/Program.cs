using System;
using System.Diagnostics;
using ElasticsearchCRUD;
using ElasticsearchCRUD.Tracing;

namespace ConsoleElasticsearchCrudExample
{
	class Program
	{
		static void Main(string[] args)
		{
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			// You only require a mapping if the default settings are not good enough
			//elasticSearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Skill), new SkillElasticSearchMapping());

			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new TraceProvider("tracingExample");
				elasticSearchContext.AddUpdateEntity(TestData.SkillEf, TestData.SkillEf.Id);
				elasticSearchContext.AddUpdateEntity(TestData.SkillOrm, TestData.SkillOrm.Id);
				elasticSearchContext.AddUpdateEntity(TestData.SkillSQLServer, TestData.SkillSQLServer.Id);
				elasticSearchContext.AddUpdateEntity(TestData.SkillGermanWithFunnyLetters, TestData.SkillGermanWithFunnyLetters.Id);
				elasticSearchContext.AddUpdateEntity(TestData.SkillLevel, TestData.SkillLevel.Id);

				var addEntitiesResult = elasticSearchContext.SaveChanges();

				Console.WriteLine(addEntitiesResult.PayloadResult);
				Console.WriteLine(addEntitiesResult.Status);
				Console.WriteLine(addEntitiesResult.Description);
			}

			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new TraceProvider("tracingExample");
				// get a entity and update it, then delete an entity
				Skill singleEntityWithId = elasticSearchContext.GetEntity<Skill>("11");
				singleEntityWithId.Updated = DateTime.UtcNow;
				elasticSearchContext.AddUpdateEntity(TestData.SkillOrm, TestData.SkillOrm.Id);
				elasticSearchContext.DeleteEntity<Skill>(TestData.SkillEf.Id);
				elasticSearchContext.SaveChanges();

				elasticSearchContext.AddUpdateEntity(TestData.SkillEf, TestData.SkillEf.Id);
				var nextResult = elasticSearchContext.SaveChanges();

				Console.WriteLine(nextResult.PayloadResult);
				Console.WriteLine(nextResult.Status);
				Console.WriteLine(nextResult.Description);
			}

			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new TraceProvider("tracingExample");
				// deleting indexes are usually not required...
				elasticSearchContext.AllowDeleteForIndex = true;
				var result = elasticSearchContext.DeleteIndexAsync<SkillLevel>();
				result.Wait();
				//var result1 = elasticSearchContext.DeleteIndexAsync<Skill>();
				//result1.Wait();
				//var result = elasticSearchContext.DeleteIndex<Skill>();
				elasticSearchContext.SaveChanges();
				//Console.WriteLine(result.Result.PayloadResult);
				//Console.WriteLine(result.Result.Status);
				//Console.WriteLine(result.Result.Description);
				//Console.ReadLine();
			}
		}
	}
}
