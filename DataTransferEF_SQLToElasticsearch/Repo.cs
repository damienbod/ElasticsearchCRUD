using System;
using System.Diagnostics;
using System.Linq;
using DataTransferSQLToEl.SQLDomainModel;
using ElasticsearchCRUD;

namespace DataTransferSQLToEl
{
	public class Repo
	{
		public Person GetPersonFromElasticsearch(int id)
		{
			Person person; 
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				person = elasticSearchContext.GetEntity<Person>(id);
			}

			return person;
		}

		public CountryRegion GetCountryRegionFromElasticsearch(string id)
		{
			CountryRegion countryRegion;
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				countryRegion = elasticSearchContext.GetEntity<CountryRegion>(id);
			}

			return countryRegion;
		}

		public void SaveToElasticsearchCountryRegion()
		{
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				//elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				using (var databaseEfModel = new SQLDataModel())
				{
					int pointer = 0;
					const int interval = 1000;
					int length = databaseEfModel.CountryRegion.Count();

					while (pointer < length)
					{
						stopwatch.Start();
						var collection = databaseEfModel.CountryRegion.OrderBy(t => t.CountryRegionCode).Skip(pointer).Take(interval).ToList<CountryRegion>();
						stopwatch.Stop();
						Console.WriteLine("Time taken for select {0} CountryRegionCode: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();

						stopwatch.Start();
						foreach (var item in collection)
						{
							elasticSearchContext.AddUpdateEntity(item, item.CountryRegionCode);
							string t = "yes";
						}
						elasticSearchContext.SaveChanges();
						stopwatch.Stop();
						Console.WriteLine("Time taken to insert {0} CountryRegionCode documents: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();						
						pointer = pointer + 1000;
						Console.WriteLine("Transferred: {0} items", pointer);
					}
				}
			}
		}

		private Stopwatch stopwatch = new Stopwatch();
		public void SaveToElasticsearchPerson()
		{
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				//elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				using (var databaseEfModel = new SQLDataModel())
				{
					int pointer = 0;
					const int interval = 1000;
					int length = databaseEfModel.Person.Count();

					while (pointer < length)
					{
						stopwatch.Start();
						var collection = databaseEfModel.Person.OrderBy(t => t.BusinessEntityID).Skip(pointer).Take(interval).ToList<Person>();
						stopwatch.Stop();
						Console.WriteLine("Time taken for select {0} persons: {1}", interval,stopwatch.Elapsed);
						stopwatch.Reset();

						stopwatch.Start();
						foreach (var item in collection)
						{
							elasticSearchContext.AddUpdateEntity(item, item.BusinessEntityID);
							string t = "yes";
						}
						elasticSearchContext.SaveChanges();
						stopwatch.Stop();
						Console.WriteLine("Time taken to insert {0} person documents: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();						
						pointer = pointer + 1000;
						Console.WriteLine("Transferred: {0} items", pointer);
					}
				}
			}
		}
	}
}
