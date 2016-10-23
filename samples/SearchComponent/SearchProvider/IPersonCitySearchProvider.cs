namespace SearchComponent
{
    public interface IPersonCitySearchProvider
    {
        void CreateIndex();

        void CreateTestData();

        void AutocompleteSearch();

        void Search(string term);
    }
}