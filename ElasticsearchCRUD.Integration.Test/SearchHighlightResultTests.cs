using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ElasticsearchCRUD.ContextSearch.SearchModel;
using NUnit.Framework;

namespace ElasticsearchCRUD.Integration.Test
{
	[TestFixture]
	public class SearchHighlightResultTests
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
		private const string ConnectionString = "http://localhost:9200";

		[TearDown]
		public void TearDown()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				context.AllowDeleteForIndex = true;

				var entityResult = context.DeleteIndexAsync<FastestAnimal>();
				entityResult.Wait();
			}
		}

		[Test]
		public void SearchForDataWithHightlightResult()
		{
			AddSearchHightlightResultData();
			Thread.Sleep(1000);
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
				var hits = context.Search<FastestAnimal>(SearchQuery("mph"), new SearchUrlParameters{Pretty=true});

				//"speedmph" : [ "60.0 <em>mph</em>"
				string value = hits.PayloadResult.Hits.HitsResult.FirstOrDefault().Highlights["speedmph"].FirstOrDefault();				
				Assert.AreEqual("60.0 <em>mph</em>", value);
			}
		}

		//{
		//	"query" :  { "match" : {   "_all" : "cool"  }},
		//	"highlight": {
		//		"fields" : {
		//			"name" : {},
		//			"description" : {}
		//		}
		//	}
		//}
		private string SearchQuery(string text)
		{
			return "{\"query\" :  { \"match\" : {   \"_all\" : \""+ text +"\"  }},\"highlight\": { \"fields\" : { \"data\" : {}, \"speed\" : {}, \"speedmph\" : {}, \"speedkmh\" : {}}}}";
		}
		private void AddSearchHightlightResultData()
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
			{
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
}
