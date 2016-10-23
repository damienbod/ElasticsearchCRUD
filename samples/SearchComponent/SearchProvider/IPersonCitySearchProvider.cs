using System.Collections.Generic;

namespace SearchComponent
{
    public interface IPersonCitySearchProvider
    {
        void CreateIndex();

        void CreateTestData();

        IEnumerable<string> AutocompleteSearch(string term);

        void Search(string term);
    }
}