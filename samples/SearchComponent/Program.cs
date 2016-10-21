using System;

namespace SearchComponent
{
    /// <summary>
    /// http://www.bilyachat.com/2015/07/search-like-google-with-elasticsearch.html
    /// Implementing solution from http://www.bilyachat.com using ElaticsearchCrud
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var personCitySearchProvider = new PersonCitySearchProvider();

            // create a new index and type mapping in elasticseach
            personCitySearchProvider.CreateIndex();
            //Console.ReadLine();

            // create a new index and type mapping in elasticseach
            personCitySearchProvider.CreateMapping();
            Console.ReadLine();
        }
    }
}
