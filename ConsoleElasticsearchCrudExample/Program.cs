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
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			// You only require a mapping if the default settings are not good enough
			//elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Skill), new SkillElasticsearchMapping());

			using (var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", elasticsearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new TraceProvider("tracingExample");
				elasticSearchContext.AddUpdateDocument(TestData.SkillEf, TestData.SkillEf.Id);
				elasticSearchContext.AddUpdateDocument(TestData.SkillOrm, TestData.SkillOrm.Id);
				elasticSearchContext.AddUpdateDocument(TestData.SkillSQLServer, TestData.SkillSQLServer.Id);
				elasticSearchContext.AddUpdateDocument(TestData.SkillGermanWithFunnyLetters, TestData.SkillGermanWithFunnyLetters.Id);
				elasticSearchContext.AddUpdateDocument(TestData.SkillLevel, TestData.SkillLevel.Id);

				var addEntitiesResult = elasticSearchContext.SaveChanges();

				Console.WriteLine(addEntitiesResult.PayloadResult);
				Console.WriteLine(addEntitiesResult.Status);
				Console.WriteLine(addEntitiesResult.Description);
			}

			using (var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", elasticsearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new TraceProvider("tracingExample");
				// get a entity and update it, then delete an entity
				Skill singleEntityWithId = elasticSearchContext.GetDocument<Skill>("11");
				singleEntityWithId.Updated = DateTime.UtcNow;
				elasticSearchContext.AddUpdateDocument(TestData.SkillOrm, TestData.SkillOrm.Id);
				elasticSearchContext.DeleteDocument<Skill>(TestData.SkillEf.Id);
				elasticSearchContext.SaveChanges();

				elasticSearchContext.AddUpdateDocument(TestData.SkillEf, TestData.SkillEf.Id);
				var nextResult = elasticSearchContext.SaveChanges();

				Console.WriteLine(nextResult.PayloadResult);
				Console.WriteLine(nextResult.Status);
				Console.WriteLine(nextResult.Description);
			}

			using (var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", elasticsearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new TraceProvider("tracingExample");
				// deleting indexes are usually not required...
				elasticSearchContext.AllowDeleteForIndex = true;
				var result = elasticSearchContext.DeleteIndexAsync<SkillLevel>();
				result.Wait();
				var result1 = elasticSearchContext.DeleteIndexAsync<Skill>();
				result1.Wait();
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
