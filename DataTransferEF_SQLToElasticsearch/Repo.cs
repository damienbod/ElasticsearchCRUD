using System;
using System.Diagnostics;
using System.Linq;
using DataTransferSQLToEl.SQLDomainModel;
using ElasticsearchCRUD;
using ElasticsearchCRUD.Tracing;

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
				elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				person = elasticSearchContext.GetEntity<Person>(id);
			}

			return person;
		}

		public Address GetAddressFromElasticsearch(string id)
		{
			Address countryRegion;
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			using (var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", elasticSearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				countryRegion = elasticSearchContext.GetEntity<Address>(id);
			}

			return countryRegion;
		}

		public void SaveToElasticsearchAddress()
		{
			IElasticSearchMappingResolver elasticSearchMappingResolver = new ElasticSearchMappingResolver();
			using ( var elasticSearchContext = new ElasticSearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(elasticSearchMappingResolver,true,true)))
			{
				elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				using (var databaseEfModel = new SQLDataModel())
				{
					int pointer = 0;
					const int interval = 100;
					int length = databaseEfModel.Address.Count();

					while (pointer < length)
					{
						stopwatch.Start();
						var collection = databaseEfModel.Address.OrderBy(t => t.AddressID).Skip(pointer).Take(interval).ToList<Address>();
						stopwatch.Stop();
						Console.WriteLine("Time taken for select {0} Address: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();

						stopwatch.Start();
						foreach (var item in collection)
						{
							var ee = item.StateProvince.CountryRegion.Name;
							elasticSearchContext.AddUpdateEntity(item, item.AddressID);
						}
						elasticSearchContext.SaveChangesAndInitMappingsForChildDocuments();
						stopwatch.Stop();
						Console.WriteLine("Time taken to insert {0} Address documents: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();
						pointer = pointer + interval;
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
						}
						elasticSearchContext.SaveChanges();
						stopwatch.Stop();
						Console.WriteLine("Time taken to insert {0} person documents: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();
						pointer = pointer + interval;
						Console.WriteLine("Transferred: {0} items", pointer);
					}
				}
			}
		}
	}
}
