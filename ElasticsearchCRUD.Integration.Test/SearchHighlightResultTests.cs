using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class SearchHighlightResultTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost.fiddler:9200";

		[Test]
		public void SearchForDataWithHightlightResult()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"), 
						new HighlightField("speed"), 
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimal>(search, new SearchUrlParameters { Pretty = true });

				//"speedmph" : [ "60.0 <em>mph</em>"]
				string value = hits.PayloadResult.Hits.HitsResult[0].Highlights["speedmph"].FirstOrDefault();
				Assert.AreEqual("60.0 <em>mph</em>", value);
			}
		}

		[Test]
		public void SearchForDataWithHightlightDifferentTagsResult()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"){PreTags= new List<string>{"<h1>"}, PostTags = new List<string>{"</h1>"}}, 
						new HighlightField("speed"){PreTags= new List<string>{"<h2>"}, PostTags = new List<string>{"</h2>"}},  
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
				{
					PreTags = new List<string> { "<h3>" },
					PostTags = new List<string> { "</h3>" },
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimal>(search, new SearchUrlParameters { Pretty = true });
				var highlightResults = hits.PayloadResult.Hits.HitsResult[2].Highlights;

				var highlightResultsSpeedmph = highlightResults["speedmph"];
				var highlightResultsData = highlightResults["data"];
				var highlightResultsSpeed = highlightResults["speed"];

				foreach (var item in highlightResultsSpeedmph)
				{
					StringAssert.Contains("<h3>", item);
				}

				foreach (var item in highlightResultsSpeed)
				{
					StringAssert.Contains("<h2>", item);
				}

				foreach (var item in highlightResultsData)
				{
					StringAssert.Contains("<h1>", item);
				}
			}
		}

		[Test]
		public void SearchForDataWithHightlightResultPostings()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"), 
						new HighlightField("speed"), 
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimalPostings>(search, new SearchUrlParameters { Pretty = true });

				//"speedmph" : [ "60.0 <em>mph</em>"]
				string value = hits.PayloadResult.Hits.HitsResult[0].Highlights["speedmph"].FirstOrDefault();
				Assert.AreEqual("60.0 <em>mph</em>", value);
			}
		}

		[Test]
		public void SearchForDataWithHightlightDifferentTagsResultPostings()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"){PreTags= new List<string>{"<h1>"}, PostTags = new List<string>{"</h1>"}}, 
						new HighlightField("speed"){PreTags= new List<string>{"<h2>"}, PostTags = new List<string>{"</h2>"}},  
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
				{
					PreTags = new List<string> { "<h3>" },
					PostTags = new List<string> { "</h3>" },
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimalPostings>(search, new SearchUrlParameters { Pretty = true });
				var highlightResults = hits.PayloadResult.Hits.HitsResult[2].Highlights;

				var highlightResultsSpeedmph = highlightResults["speedmph"];
				var highlightResultsData = highlightResults["data"];
				var highlightResultsSpeed = highlightResults["speed"];

				foreach (var item in highlightResultsSpeedmph)
				{
					StringAssert.Contains("<h3>", item);
				}

				foreach (var item in highlightResultsSpeed)
				{
					StringAssert.Contains("<h2>", item);
				}

				foreach (var item in highlightResultsData)
				{
					StringAssert.Contains("<h1>", item);
				}
			}
		}

		[Test]
		public void SearchForDataWithHightlightResultFastVectorHighlighter()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"), 
						new HighlightField("speed"), 
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimalFastVectorHighlighter>(search, new SearchUrlParameters { Pretty = true });

				//"speedmph" : [ "60.0 <em>mph</em>"]
				string value = hits.PayloadResult.Hits.HitsResult[0].Highlights["speedmph"].FirstOrDefault();
				Assert.AreEqual("60.0 <em>mph</em>", value);
			}
		}

		[Test]
		public void SearchForDataWithHightlightDifferentTagsResultFastestAnimalFastVectorHighlighter()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"){PreTags= new List<string>{"<h1>"}, PostTags = new List<string>{"</h1>"}}, 
						new HighlightField("speed"){PreTags= new List<string>{"<h2>"}, PostTags = new List<string>{"</h2>"}},  
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
				{
					PreTags = new List<string> { "<h3>" },
					PostTags = new List<string> { "</h3>" },
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimalFastVectorHighlighter>(search, new SearchUrlParameters { Pretty = true });
				var highlightResults = hits.PayloadResult.Hits.HitsResult[2].Highlights;

				var highlightResultsSpeedmph = highlightResults["speedmph"];
				var highlightResultsData = highlightResults["data"];
				var highlightResultsSpeed = highlightResults["speed"];

				foreach (var item in highlightResultsSpeedmph)
				{
					StringAssert.Contains("<h3>", item);
				}

				foreach (var item in highlightResultsSpeed)
				{
					StringAssert.Contains("<h2>", item);
				}

				foreach (var item in highlightResultsData)
				{
					StringAssert.Contains("<h1>", item);
				}
			}
		}

		[Test]
		public void SearchForDataWithHightlightResultFastVectorHighlighterMultipleTags()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all","mph km"){Operator = Operator.or}),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data")
						{
							ForceSource= true, 
							Encoder = HighlightEncoder.@default, 
							FieldType = HighlightFieldType.fvh, 
							PreTags = new List<string> { "<h4>" },
							PostTags =  new List<string> { "</h4>" }, 
							FragmentOffset = 2, 
							FragmentSize = 100, 
							NumberOfFragments = 20, 
							OrderByScore = true, 
							NoMatchSize = 3
						}, 
						new HighlightField("speed"){MatchedFields = new List<string>{"speedmph", "speedkmh" }}, 
						new HighlightField("speedmph"){FieldType = HighlightFieldType.plain},
						new HighlightField("_all")
					})
				{
					PreTags = new List<string> { "<h1>", "<h2>", "<h3>" },
					PostTags =  new List<string> { "</h1>", "</h2>", "</h3>" }, BoundaryMaxScan = 3
					
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimalFastVectorHighlighter>(search, new SearchUrlParameters { Pretty = true });

				//"speedmph" : [ "60.0 <em>mph</em>"]
				string value = hits.PayloadResult.Hits.HitsResult[0].Highlights["speedmph"].FirstOrDefault();
				Assert.AreEqual("60.0 <h1>mph</h1>", value);
			}
		}

		[Test]
		public void SearchForDataWithHightlightResultFastVectorHighlighterStyled()
		{
			var search = new Search
			{
				Query = new Query(new MatchQuery("_all", "mph")),
				Highlight = new Highlight(
					new List<HighlightField>
					{
						new HighlightField("data"), 
						new HighlightField("speed"), 
						new HighlightField("speedmph"),
						new HighlightField("speedkmh")
					})
				{
					TagsSchemaStyled= true
				}
			};

			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimalFastVectorHighlighter>(search, new SearchUrlParameters { Pretty = true });

				//"speedmph" : [ "60.0 <em>mph</em>"]
				string value = hits.PayloadResult.Hits.HitsResult[0].Highlights["speedmph"].FirstOrDefault();
				Assert.AreEqual("60.0 <em class=\"hlt1\">mph</em>", value);
			}
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;

				if (context.IndexExists<FastestAnimal>())
				{
					var entityResult1 = context.DeleteIndexAsync<FastestAnimal>();
					entityResult1.Wait();
				}

				if (context.IndexExists<FastestAnimalPostings>())
				{
					var entityResult2 = context.DeleteIndexAsync<FastestAnimalPostings>();
					entityResult2.Wait();
				}

				if (context.IndexExists<FastestAnimalFastVectorHighlighter>())
				{
					var entityResult3 = context.DeleteIndexAsync<FastestAnimalFastVectorHighlighter>();
					entityResult3.Wait();
				}
			}
		}

		[TestFixtureSetUp]
		public void SetupFixture()
		{
			AddSearchHightlightResultData();
			AddSearchHightlightResultDataFastestAnimalPostings();
			AddSearchHightlightResultDataFastestAnimalFastVectorHighlighter();
			Thread.Sleep(1200);
		}

		private void AddSearchHightlightResultData()
		{			
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<FastestAnimal>();
				Thread.Sleep(1200);
				var animals = new List<FastestAnimal>
				{
					new FastestAnimal {Id = 1, AnimalName = "Cheetah", Speed = "112–120 km/h (70–75 mph)", SpeedMph="70–75 mph", SpeedKmh="112–120 km/h", Data = "The Cheetah can accelerate from 0 to 96.6 km/h (60.0 mph) in under three seconds, though endurance is limited: most Cheetahs run for only 60 seconds at a time. When sprinting, cheetahs spend more time in the air than on the ground."},
					new FastestAnimal {Id = 2, AnimalName = "Free-tailed bat", Speed = "96.6 km/h (60.0 mph)",  SpeedMph="60.0 mph", SpeedKmh="96.6 km/h",Data = "Some attribute such flying capabilities specifically to the Mexican free-tailed bat. Tail wind is what allows free-tailed bats to reach such high speeds."},
					new FastestAnimal {Id = 3, AnimalName = "Pronghorn", Speed = "96.6 km/h (60.0 mph)",  SpeedMph="60.0 mph", SpeedKmh="96.6 km/h",Data = "The pronghorn (American antelope) is the fastest animal over long distances; it can run 56 km/h for 6 km (35 mph for 4 mi), 67 km/h for 1.6 km (42 mph for 1 mi), and 88.5 km/h for .8 km (55 mph for .5 mi)."},
					new FastestAnimal {Id = 4, AnimalName = "Springbok", Speed = "88 km/h (55 mph)",  SpeedMph="55 mph", SpeedKmh="88 km/h",Data = "The springbok, an antelope of the gazelle tribe in southern Africa, can make long jumps and sharp turns while running. Unlike pronghorns, springboks are poor long-distance runners."},
					new FastestAnimal {Id = 5, AnimalName = "Wildebeest", Speed = "80.5 km/h (50.0 mph)",  SpeedMph="50.0 mph", SpeedKmh="80.5 km/h",Data = "The wildebeest, an antelope, exists as two species: the blue wildebeest and the black wildebeest. Both are extremely fast runners, which allows them to flee from predators. They are better at endurance running than at sprinting."},
					new FastestAnimal {Id = 6, AnimalName = "Blackbuck", Speed = "80 km/h (50 mph)",  SpeedMph="50 mph", SpeedKmh="80 km/h",Data = "The blackbuck antelope can sustain speeds of 80 km/h (50 mph) for over 1.5 km (0.93 mi) at a time. Each of its strides (i.e., the distance between its hoofprints) is 5.8–6.7 m (19–22 ft)."},
					new FastestAnimal {Id = 7, AnimalName = "Lion", Speed = "80 km/h (50 mph)",  SpeedMph="50 mph", SpeedKmh="80 km/h",Data = "Lionesses are faster than males and can reach maximum speeds of 35 mph (57 km/h) in short distances of approximately 90 meters, and a top speed of 50 mph (80 km/h) for about 20 meters. Lions are very agile and have fast reflexes. Like other predators, they hunt sick prey. Their rate of success in hunting is greatest at night. Lions hunt buffalos, giraffes, warthogs, wildebeests and zebras, and sometimes various antelopes as opportunities present themselves."},
					new FastestAnimal {Id = 8, AnimalName = "Greyhound", Speed = "74 km/h (46 mph)",  SpeedMph="46 mph", SpeedKmh="74 km/h",Data = "Greyhounds are the fastest dogs, and have primarily been bred for coursing game and racing."},
					new FastestAnimal {Id = 9, AnimalName = "Jackrabbit", Speed = "72 km/h (45 mph)",  SpeedMph="45 mph", SpeedKmh="72 km/h",Data = "The jackrabbit's strong hind legs allow it to leap 3 m (9.8 ft) in one bound; some can even reach 6 m (20 ft). Jackrabbits use a combination of leaps and zig-zags to outrun predators."},
					new FastestAnimal {Id = 10, AnimalName = "African wild dog", Speed = "71 km/h (44 mph)",  SpeedMph="44 mph", SpeedKmh="71 km/h",Data = "When hunting, African wild dogs can sprint at 66 km/h (41 mph) in bursts, and they can maintain speeds of 56–60 km/h (35–37 mph) for up to 4.8 km (3 mi). Their targeted prey rarely escapes."},
					new FastestAnimal {Id = 11, AnimalName = "Kangaroo", Speed = "71 km/h (44 mph)",  SpeedMph="44 mph", SpeedKmh="71 km/h",Data = "The comfortable hopping speed for a kangaroo is about 21–26 km/h (13–16 mph), but speeds of up to 71 km/h (44 mph) can be attained over short distances, while it can sustain a speed of 40 km/h (25 mph) for nearly 2 km (1.2 mi). The faster a kangaroo hops, the less energy it consumes (up to its cruising speed)."},
					new FastestAnimal {Id = 12, AnimalName = "Horse", Speed = "70.76 km/h (43.97 mph)",  SpeedMph="43.97 mph", SpeedKmh="70.76 km/h",Data = "The fastest horse speed was achieved by a Quarter horse. It reached 70.76 km/h (43.97 mph)."},
					new FastestAnimal {Id = 13, AnimalName = "Onager", Speed = "70 km/h (43 mph)",  SpeedMph="43 mph", SpeedKmh="70 km/h",Data = "The onager consists of several subspecies, which most likely share the same ability to run at high speeds."},
					new FastestAnimal {Id = 14, AnimalName = "Thomson's gazelle", Speed = "70 km/h (43 mph)",  SpeedMph="43 mph", SpeedKmh="70 km/h",Data = "Thomson's gazelles, being long-distance runners, can escape cheetahs by sheer endurance. Their speed is partially due to their \"stotting\", or bounding leaps."},
				};

				foreach (var animal in animals)
				{
					context.AddUpdateDocument(animal, animal.Id);
				}
				
				context.SaveChanges();
			}
			
		}

		private void AddSearchHightlightResultDataFastestAnimalPostings()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<FastestAnimalPostings>();
				Thread.Sleep(1200);

				var animals = new List<FastestAnimalPostings>
				{
					new FastestAnimalPostings {Id = 1, AnimalName = "Cheetah", Speed = "112–120 km/h (70–75 mph)", SpeedMph="70–75 mph", SpeedKmh="112–120 km/h", Data = "The Cheetah can accelerate from 0 to 96.6 km/h (60.0 mph) in under three seconds, though endurance is limited: most Cheetahs run for only 60 seconds at a time. When sprinting, cheetahs spend more time in the air than on the ground."},
					new FastestAnimalPostings {Id = 2, AnimalName = "Free-tailed bat", Speed = "96.6 km/h (60.0 mph)",  SpeedMph="60.0 mph", SpeedKmh="96.6 km/h",Data = "Some attribute such flying capabilities specifically to the Mexican free-tailed bat. Tail wind is what allows free-tailed bats to reach such high speeds."},
					new FastestAnimalPostings {Id = 3, AnimalName = "Pronghorn", Speed = "96.6 km/h (60.0 mph)",  SpeedMph="60.0 mph", SpeedKmh="96.6 km/h",Data = "The pronghorn (American antelope) is the fastest animal over long distances; it can run 56 km/h for 6 km (35 mph for 4 mi), 67 km/h for 1.6 km (42 mph for 1 mi), and 88.5 km/h for .8 km (55 mph for .5 mi)."},
					new FastestAnimalPostings {Id = 4, AnimalName = "Springbok", Speed = "88 km/h (55 mph)",  SpeedMph="55 mph", SpeedKmh="88 km/h",Data = "The springbok, an antelope of the gazelle tribe in southern Africa, can make long jumps and sharp turns while running. Unlike pronghorns, springboks are poor long-distance runners."},
					new FastestAnimalPostings {Id = 5, AnimalName = "Wildebeest", Speed = "80.5 km/h (50.0 mph)",  SpeedMph="50.0 mph", SpeedKmh="80.5 km/h",Data = "The wildebeest, an antelope, exists as two species: the blue wildebeest and the black wildebeest. Both are extremely fast runners, which allows them to flee from predators. They are better at endurance running than at sprinting."},
					new FastestAnimalPostings {Id = 6, AnimalName = "Blackbuck", Speed = "80 km/h (50 mph)",  SpeedMph="50 mph", SpeedKmh="80 km/h",Data = "The blackbuck antelope can sustain speeds of 80 km/h (50 mph) for over 1.5 km (0.93 mi) at a time. Each of its strides (i.e., the distance between its hoofprints) is 5.8–6.7 m (19–22 ft)."},
					new FastestAnimalPostings {Id = 7, AnimalName = "Lion", Speed = "80 km/h (50 mph)",  SpeedMph="50 mph", SpeedKmh="80 km/h",Data = "Lionesses are faster than males and can reach maximum speeds of 35 mph (57 km/h) in short distances of approximately 90 meters, and a top speed of 50 mph (80 km/h) for about 20 meters. Lions are very agile and have fast reflexes. Like other predators, they hunt sick prey. Their rate of success in hunting is greatest at night. Lions hunt buffalos, giraffes, warthogs, wildebeests and zebras, and sometimes various antelopes as opportunities present themselves."},
					new FastestAnimalPostings {Id = 8, AnimalName = "Greyhound", Speed = "74 km/h (46 mph)",  SpeedMph="46 mph", SpeedKmh="74 km/h",Data = "Greyhounds are the fastest dogs, and have primarily been bred for coursing game and racing."},
					new FastestAnimalPostings {Id = 9, AnimalName = "Jackrabbit", Speed = "72 km/h (45 mph)",  SpeedMph="45 mph", SpeedKmh="72 km/h",Data = "The jackrabbit's strong hind legs allow it to leap 3 m (9.8 ft) in one bound; some can even reach 6 m (20 ft). Jackrabbits use a combination of leaps and zig-zags to outrun predators."},
					new FastestAnimalPostings {Id = 10, AnimalName = "African wild dog", Speed = "71 km/h (44 mph)",  SpeedMph="44 mph", SpeedKmh="71 km/h",Data = "When hunting, African wild dogs can sprint at 66 km/h (41 mph) in bursts, and they can maintain speeds of 56–60 km/h (35–37 mph) for up to 4.8 km (3 mi). Their targeted prey rarely escapes."},
					new FastestAnimalPostings {Id = 11, AnimalName = "Kangaroo", Speed = "71 km/h (44 mph)",  SpeedMph="44 mph", SpeedKmh="71 km/h",Data = "The comfortable hopping speed for a kangaroo is about 21–26 km/h (13–16 mph), but speeds of up to 71 km/h (44 mph) can be attained over short distances, while it can sustain a speed of 40 km/h (25 mph) for nearly 2 km (1.2 mi). The faster a kangaroo hops, the less energy it consumes (up to its cruising speed)."},
					new FastestAnimalPostings {Id = 12, AnimalName = "Horse", Speed = "70.76 km/h (43.97 mph)",  SpeedMph="43.97 mph", SpeedKmh="70.76 km/h",Data = "The fastest horse speed was achieved by a Quarter horse. It reached 70.76 km/h (43.97 mph)."},
					new FastestAnimalPostings {Id = 13, AnimalName = "Onager", Speed = "70 km/h (43 mph)",  SpeedMph="43 mph", SpeedKmh="70 km/h",Data = "The onager consists of several subspecies, which most likely share the same ability to run at high speeds."},
					new FastestAnimalPostings {Id = 14, AnimalName = "Thomson's gazelle", Speed = "70 km/h (43 mph)",  SpeedMph="43 mph", SpeedKmh="70 km/h",Data = "Thomson's gazelles, being long-distance runners, can escape cheetahs by sheer endurance. Their speed is partially due to their \"stotting\", or bounding leaps."},
				};

				foreach (var animal in animals)
				{
					context.AddUpdateDocument(animal, animal.Id);
				}

				context.SaveChanges();
			}

		}

		private void AddSearchHightlightResultDataFastestAnimalFastVectorHighlighter()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.IndexCreate<FastestAnimalFastVectorHighlighter>();
				Thread.Sleep(1200);

				var animals = new List<FastestAnimalFastVectorHighlighter>
				{
					new FastestAnimalFastVectorHighlighter {Id = 1, AnimalName = "Cheetah", Speed = "112–120 km/h (70–75 mph)", SpeedMph="70–75 mph", SpeedKmh="112–120 km/h", Data = "The Cheetah can accelerate from 0 to 96.6 km/h (60.0 mph) in under three seconds, though endurance is limited: most Cheetahs run for only 60 seconds at a time. When sprinting, cheetahs spend more time in the air than on the ground."},
					new FastestAnimalFastVectorHighlighter {Id = 2, AnimalName = "Free-tailed bat", Speed = "96.6 km/h (60.0 mph)",  SpeedMph="60.0 mph", SpeedKmh="96.6 km/h",Data = "Some attribute such flying capabilities specifically to the Mexican free-tailed bat. Tail wind is what allows free-tailed bats to reach such high speeds."},
					new FastestAnimalFastVectorHighlighter {Id = 3, AnimalName = "Pronghorn", Speed = "96.6 km/h (60.0 mph)",  SpeedMph="60.0 mph", SpeedKmh="96.6 km/h",Data = "The pronghorn (American antelope) is the fastest animal over long distances; it can run 56 km/h for 6 km (35 mph for 4 mi), 67 km/h for 1.6 km (42 mph for 1 mi), and 88.5 km/h for .8 km (55 mph for .5 mi)."},
					new FastestAnimalFastVectorHighlighter {Id = 4, AnimalName = "Springbok", Speed = "88 km/h (55 mph)",  SpeedMph="55 mph", SpeedKmh="88 km/h",Data = "The springbok, an antelope of the gazelle tribe in southern Africa, can make long jumps and sharp turns while running. Unlike pronghorns, springboks are poor long-distance runners."},
					new FastestAnimalFastVectorHighlighter {Id = 5, AnimalName = "Wildebeest", Speed = "80.5 km/h (50.0 mph)",  SpeedMph="50.0 mph", SpeedKmh="80.5 km/h",Data = "The wildebeest, an antelope, exists as two species: the blue wildebeest and the black wildebeest. Both are extremely fast runners, which allows them to flee from predators. They are better at endurance running than at sprinting."},
					new FastestAnimalFastVectorHighlighter {Id = 6, AnimalName = "Blackbuck", Speed = "80 km/h (50 mph)",  SpeedMph="50 mph", SpeedKmh="80 km/h",Data = "The blackbuck antelope can sustain speeds of 80 km/h (50 mph) for over 1.5 km (0.93 mi) at a time. Each of its strides (i.e., the distance between its hoofprints) is 5.8–6.7 m (19–22 ft)."},
					new FastestAnimalFastVectorHighlighter {Id = 7, AnimalName = "Lion", Speed = "80 km/h (50 mph)",  SpeedMph="50 mph", SpeedKmh="80 km/h",Data = "Lionesses are faster than males and can reach maximum speeds of 35 mph (57 km/h) in short distances of approximately 90 meters, and a top speed of 50 mph (80 km/h) for about 20 meters. Lions are very agile and have fast reflexes. Like other predators, they hunt sick prey. Their rate of success in hunting is greatest at night. Lions hunt buffalos, giraffes, warthogs, wildebeests and zebras, and sometimes various antelopes as opportunities present themselves."},
					new FastestAnimalFastVectorHighlighter {Id = 8, AnimalName = "Greyhound", Speed = "74 km/h (46 mph)",  SpeedMph="46 mph", SpeedKmh="74 km/h",Data = "Greyhounds are the fastest dogs, and have primarily been bred for coursing game and racing."},
					new FastestAnimalFastVectorHighlighter {Id = 9, AnimalName = "Jackrabbit", Speed = "72 km/h (45 mph)",  SpeedMph="45 mph", SpeedKmh="72 km/h",Data = "The jackrabbit's strong hind legs allow it to leap 3 m (9.8 ft) in one bound; some can even reach 6 m (20 ft). Jackrabbits use a combination of leaps and zig-zags to outrun predators."},
					new FastestAnimalFastVectorHighlighter {Id = 10, AnimalName = "African wild dog", Speed = "71 km/h (44 mph)",  SpeedMph="44 mph", SpeedKmh="71 km/h",Data = "When hunting, African wild dogs can sprint at 66 km/h (41 mph) in bursts, and they can maintain speeds of 56–60 km/h (35–37 mph) for up to 4.8 km (3 mi). Their targeted prey rarely escapes."},
					new FastestAnimalFastVectorHighlighter {Id = 11, AnimalName = "Kangaroo", Speed = "71 km/h (44 mph)",  SpeedMph="44 mph", SpeedKmh="71 km/h",Data = "The comfortable hopping speed for a kangaroo is about 21–26 km/h (13–16 mph), but speeds of up to 71 km/h (44 mph) can be attained over short distances, while it can sustain a speed of 40 km/h (25 mph) for nearly 2 km (1.2 mi). The faster a kangaroo hops, the less energy it consumes (up to its cruising speed)."},
					new FastestAnimalFastVectorHighlighter {Id = 12, AnimalName = "Horse", Speed = "70.76 km/h (43.97 mph)",  SpeedMph="43.97 mph", SpeedKmh="70.76 km/h",Data = "The fastest horse speed was achieved by a Quarter horse. It reached 70.76 km/h (43.97 mph)."},
					new FastestAnimalFastVectorHighlighter {Id = 13, AnimalName = "Onager", Speed = "70 km/h (43 mph)",  SpeedMph="43 mph", SpeedKmh="70 km/h",Data = "The onager consists of several subspecies, which most likely share the same ability to run at high speeds."},
					new FastestAnimalFastVectorHighlighter {Id = 14, AnimalName = "Thomson's gazelle", Speed = "70 km/h (43 mph)",  SpeedMph="43 mph", SpeedKmh="70 km/h",Data = "Thomson's gazelles, being long-distance runners, can escape cheetahs by sheer endurance. Their speed is partially due to their \"stotting\", or bounding leaps."},
				};

				foreach (var animal in animals)
				{
					context.AddUpdateDocument(animal, animal.Id);
				}

				context.SaveChanges();
			}

		}
	}

	public class FastestAnimal
	{
		public long Id { get; set; }
		public string AnimalName { get; set; }
		public string Speed { get; set; }
		public string SpeedMph { get; set; }
		public string SpeedKmh { get; set; }
		public string Data { get; set; }
	}

	public class FastestAnimalPostings 
	{
		public long Id { get; set; }
		public string AnimalName { get; set; }

		[ElasticsearchString(IndexOptions = IndexOptions.offsets)]
		public string Speed { get; set; }

		[ElasticsearchString(IndexOptions = IndexOptions.offsets)]
		public string SpeedMph { get; set; }

		[ElasticsearchString(IndexOptions = IndexOptions.offsets)]
		public string SpeedKmh { get; set; }

		[ElasticsearchString(IndexOptions = IndexOptions.offsets)]
		public string Data { get; set; }
	}

	public class FastestAnimalFastVectorHighlighter
	{
		public long Id { get; set; }
		public string AnimalName { get; set; }

		[ElasticsearchString(TermVector = TermVector.with_positions_offsets, Store = true)]
		public string Speed { get; set; }

		[ElasticsearchString(TermVector = TermVector.with_positions_offsets, Store = true)]
		public string SpeedMph { get; set; }

		[ElasticsearchString(TermVector = TermVector.with_positions_offsets, Store = true)]
		public string SpeedKmh { get; set; }

		[ElasticsearchString(TermVector = TermVector.with_positions_offsets, Store = true)]
		public string Data { get; set; }
	}
}
