using System;
using System.Collections.Generic;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Filters;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Tracing;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test.SearchTests
{
	[TestFixture]
	public class SearchQueryFunctionScoreQueries : SetupSearch
	{
		[Test]
		public void SearchQueryFunctionScoreQueryLinearNumber()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new LinearNumericFunction<double>("lift", 2.0, 3.0)
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryLinearDateTime()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new LinearDateTimePointFunction("dateofdetails", new TimeUnitDay(2))
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0,
								Origin = DateTime.UtcNow
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryLinearGeoPoint()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new LinearGeoPointFunction("location", new GeoPoint(40,40), new DistanceUnitKilometer(100) )
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryGaussNumber()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new GaussNumericFunction<double>("lift", 2.0, 3.0)
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryGaussDateTime()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new GaussDateTimePointFunction("dateofdetails", new TimeUnitDay(2))
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0,
								Origin = DateTime.UtcNow
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryGaussGeoPoint()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new GaussGeoPointFunction("location", new GeoPoint(40,40), new DistanceUnitKilometer(100) )
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryExpNumber()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new ExpNumericFunction<double>("lift", 2.0, 3.0)
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryExpDateTime()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new ExpDateTimePointFunction("dateofdetails", new TimeUnitDay(2))
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0,
								Origin = DateTime.UtcNow
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryExpGeoPoint()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new ExpGeoPointFunction("location", new GeoPoint(40,40), new DistanceUnitKilometer(100) )
							{
								Decay=0.3,
								Filter = new MatchAllFilter(),
								Offset= 3,
								Weight= 3.0
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryScriptScoreFunction()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new ScriptScoreFunction("_score * doc['lift'].value / pow(param1, param2)")
							{
								Params= new List<ScriptParameter>
								{
									new ScriptParameter("param1", 2.0),
									new ScriptParameter("param2", 2.5)
								}
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryRandomFunctionString()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new RandomScoreFunction<string>("d")
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryRandomFunctionLong()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new RandomScoreFunction<long>(4)
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}

		[Test]
		public void SearchQueryFunctionScoreQueryFieldValueFactorFunction()
		{
			var search = new Search
			{
				Query = new Query(
					new FunctionScoreQuery(
						new MatchAllQuery(),
						new List<BaseScoreFunction>
						{
							new FieldValueFactorFunction("lift")
							{
								Factor=2.3,
								Modifier= FieldValueFactorModifier.sqrt
							}
						}
					)
				)
			};

			using (var context = new ElasticsearchContext(ConnectionString, ElasticsearchMappingResolver))
			{
				context.TraceProvider = new ConsoleTraceProvider();
				Assert.IsTrue(context.IndexTypeExists<SearchTest>());
				var items = context.Search<SearchTest>(search);
				Assert.AreEqual(3, items.PayloadResult.Hits.Total);
			}
		}
		
	}

}