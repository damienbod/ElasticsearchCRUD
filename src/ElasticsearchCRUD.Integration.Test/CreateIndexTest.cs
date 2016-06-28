using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Aggregations;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Tracing;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test
{
    using System;

    public class CreateIndexTest : IDisposable
    {
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

        [Fact]
        public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayList()
		{
			using ( var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<MappingTestsParent>();

				Thread.Sleep(1500);
				var result = context.IndexExists<MappingTestsParent>();
				Assert.True(result);
			}
		}

        [Fact]
        public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayListAndAlias()
		{
			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<AliasCreateIndexOne>(
					new IndexDefinition
					{
						IndexAliases = new IndexAliases
						{
							Aliases = new List<IndexAlias>
							{
								new IndexAlias("testnew"), new IndexAlias("testnewroute")
								{
									Routing = "ddd"
								}
							}
						}
					});

				Thread.Sleep(1500);
				var result = context.IndexExists<AliasCreateIndexOne>();
				Assert.True(result);
			}
		}

        // http://localhost.fiddler:9200/aliascreateindexones/_warmers
        [Fact]
        public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayListAndAliasAndWarmer()
		{
			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<AliasCreateIndexOne>(
					new IndexDefinition
					{
						IndexAliases = new IndexAliases
						{
							Aliases = new List<IndexAlias>
							{
								new IndexAlias("testnew"), new IndexAlias("testnewroute")
								{
									Routing = "ddd"
								}
							}					
						},
						IndexWarmers = new IndexWarmers
						{
							Warmers = new List<IndexWarmer>
							{
								new IndexWarmer("warmer_one")
								{
									Query= new Query(new MatchAllQuery()),
									Aggs = new List<IAggs>
									{
										new SumMetricAggregation("sum", "id")
									}
								}
							}
						}
					});

				Thread.Sleep(1500);
				var result = context.IndexExists<AliasCreateIndexOne>();
				Assert.True(result);
			}
		}

        [Fact]
        public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayListAndAliasAnd2Warmers()
		{
			var indexDefinition = new IndexDefinition
			{
				IndexAliases = new IndexAliases
				{
					Aliases = new List<IndexAlias>
					{
						new IndexAlias("testnew"),
						new IndexAlias("testnewroute")
						{
							Routing = "ddd"
						}
					}
				},
				IndexWarmers = new IndexWarmers
				{
					Warmers = new List<IndexWarmer>
					{
						new IndexWarmer("warmer_one")
						{
							Query = new Query(new MatchAllQuery()),
							Aggs = new List<IAggs>
							{
								new SumMetricAggregation("sum", "id")
							}
						},
						new IndexWarmer("warmer_two")
						{
							Query = new Query(new MatchAllQuery()),
							IndexTypes = new List<string>
							{
								"dd",
								"ee"
							}
						}
					}
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate<AliasCreateIndexOne>(indexDefinition);

				Thread.Sleep(1500);
				var result = context.IndexExists<AliasCreateIndexOne>();
				Assert.True(result);
			}
		}

        [Fact]
        public void CreateNewIndexAndMappingWithSimpleNullListAndNullArrayListAndAlias2()
		{
			using (var context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				context.IndexCreate("aliascreateindextwos", new IndexSettings(), new IndexAliases { Aliases = new List<IndexAlias> { new IndexAlias("testhhhnew"), new IndexAlias("testnewroute") { Routing = "dhd" } } });

				Thread.Sleep(1500);
				var result = context.IndexExists<AliasCreateIndexTwo>();
				Assert.True(result);
			}
		}

        public void Dispose()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                context.AllowDeleteForIndex = true;
                if (context.IndexExists<MappingTestsParent>())
                {
                    var entityResult1 = context.DeleteIndexAsync<MappingTestsParent>();
                    entityResult1.Wait();
                }

                if (context.IndexExists<AliasCreateIndexOne>())
                {
                    var entityResult2 = context.DeleteIndexAsync<AliasCreateIndexOne>();
                    entityResult2.Wait();
                }

                if (context.IndexExists<AliasCreateIndexTwo>())
                {
                    var entityResult3 = context.DeleteIndexAsync<AliasCreateIndexTwo>();
                    entityResult3.Wait();
                }
            }
        }
    }


	public class AliasCreateIndexOne
	{
		public int Id { get; set; }

		public string data { get; set; }
	}

	public class AliasCreateIndexTwo
	{
		public int Id { get; set; }

		public string data { get; set; }
	}
}
