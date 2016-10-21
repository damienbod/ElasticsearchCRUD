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
            
            personCitySearchProvider.CreateIndex();
            Console.ReadLine();
            
            personCitySearchProvider.CreateMapping();
            Console.ReadLine();

            personCitySearchProvider.CreateTestData();
            Console.ReadLine();

            personCitySearchProvider.Search("l");
            Console.ReadLine();
        }
    }
}
