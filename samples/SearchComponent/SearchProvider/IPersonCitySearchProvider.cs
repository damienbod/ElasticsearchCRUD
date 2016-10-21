namespace SearchComponent
{
    public interface IPersonCitySearchProvider
    {
        void CreateIndex();

        void CreateTestData();

        void Search(string term);
    }
}