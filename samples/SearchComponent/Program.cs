using System;

namespace SearchComponent
{
    /// <summary>
    /// Elasticsearch code built using the following two examples:
    /// http://www.bilyachat.com/2015/07/search-like-google-with-elasticsearch.html
    /// and
    /// https://qbox.io/blog/an-introduction-to-ngrams-in-elasticsearch
    /// 
    /// Implementing solution from http://www.bilyachat.com using ElaticsearchCrud
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var personCitySearchProvider = new PersonCitySearchProvider();
            
            ////personCitySearchProvider.CreateIndex();
            ////Console.ReadLine();
            
            ////personCitySearchProvider.CreateMapping();
            ////Console.ReadLine();

            ////personCitySearchProvider.CreateTestData();
            ////Console.ReadLine();

            var autocompleteResults = personCitySearchProvider.AutocompleteSearch("l");
            foreach(var item in autocompleteResults)
            {
                Console.Write(item + " ,");
            }
            Console.ReadLine();
        }
    }
}
