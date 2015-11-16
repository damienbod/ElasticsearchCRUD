using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DataTransferSQLToEl.SQLDomainModel;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Tracing;

namespace DataTransferSQLToEl
{
	public class Repo
	{
		public Person GetPersonFromElasticsearch(int id)
		{
			Person person; 
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			using (var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", elasticsearchMappingResolver))
			{
				elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				person = elasticSearchContext.GetDocument<Person>(id);
			}

			return person;
		}

		public Address GetAddressFromElasticsearch(string id)
		{
			Address countryRegion;
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			using (var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", elasticsearchMappingResolver))
			{
				//elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				countryRegion = elasticSearchContext.GetDocument<Address>(id);
			}

			return countryRegion;
		}

		public void SaveToElasticsearchStateProvince()
		{
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			using ( var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", new ElasticsearchSerializerConfiguration(elasticsearchMappingResolver,true,true)))
			{
				//elasticSearchContext.TraceProvider = new ConsoleTraceProvider();
				elasticSearchContext.IndexCreate<StateProvince>();
				using (var databaseEfModel = new SQLDataModel())
				{
					int pointer = 0;
					const int interval = 20;
					int length = databaseEfModel.StateProvince.Count();

					while (pointer < length)
					{
						stopwatch.Start();
						var collection = databaseEfModel.StateProvince.OrderBy(t => t.StateProvinceID).Skip(pointer).Take(interval).ToList<StateProvince>();
						stopwatch.Stop();
						Console.WriteLine("Time taken for select {0} Address: {1}", interval, stopwatch.Elapsed);
						stopwatch.Reset();

						stopwatch.Start();
						foreach (var item in collection)
						{
							var ee = item.CountryRegion.Name;
							elasticSearchContext.AddUpdateDocument(item, item.StateProvinceID);
						}

						elasticSearchContext.SaveChanges();
												
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

			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			using (var elasticSearchContext = new ElasticsearchContext("http://localhost:9200/", elasticsearchMappingResolver))
			{
				if (elasticSearchContext.IndexExists<Person>())
				{
					elasticSearchContext.DeleteIndex<Person>();
					Thread.Sleep(1200);
				}

				elasticSearchContext.IndexCreate<Person>(new IndexDefinition
				{
					IndexSettings = new IndexSettings
					{
						NumberOfReplicas = 1
					}
				});

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
							elasticSearchContext.AddUpdateDocument(item, item.BusinessEntityID);
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
