using System.Collections.Generic;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;

namespace SearchComponent
{
    public class PersonCitySearchProvider : IPersonCitySearchProvider
    {
        private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
        private const string ConnectionString = "http://localhost:9200";
        private readonly ElasticsearchContext _context;

        public PersonCitySearchProvider()
        {
            _elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(PersonCityMappingDto), new PersonCityMapping());
            _context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver))
            {
                TraceProvider = new ConsoleTraceProvider()
            };
        }

        public void CreateIndex()
        {			
            _context.IndexCreate<PersonCityMappingDto>(CreateNewIndexDefinition());
        }

        public void CreateMapping()
        {
            _context.IndexCreateTypeMapping<PersonCityMappingDto>(new MappingDefinition());
        }

        private IndexDefinition CreateNewIndexDefinition()
        {
            return new IndexDefinition
            {
                IndexSettings =
                {
                    Analysis = new Analysis
                    {
                        Filters =
                        {
                            CustomFilters = new List<AnalysisFilterBase>
                            {
                                new StemmerTokenFilter("stemmer"),
                                new ShingleTokenFilter("autocompleteFilter")
                                {
                                    MaxShingleSize = 5,
                                    MinShingleSize = 2
                                },
                                new StopTokenFilter("stopwords")
                            }
                        },
                        Analyzer =
                        {
                            Analyzers = new List<AnalyzerBase>
                            {
                                new CustomAnalyzer("didYouMean")
                                {
                                    Tokenizer = DefaultTokenizers.Standard,
                                    Filter = new List<string> {DefaultTokenFilters.Lowercase},
                                    CharFilter = new List<string> {DefaultCharFilters.HtmlStrip}
                                },
                                new CustomAnalyzer("autocomplete")
                                {
                                    Tokenizer = DefaultTokenizers.Standard,
                                    Filter = new List<string> {DefaultTokenFilters.Lowercase, "autocompleteFilter"},
                                    CharFilter = new List<string> {DefaultCharFilters.HtmlStrip}
                                },
                                new CustomAnalyzer("default")
                                {
                                    Tokenizer = DefaultTokenizers.Standard,
                                    Filter = new List<string> {DefaultTokenFilters.Lowercase, "stopwords", "stemmer"},
                                    CharFilter = new List<string> {DefaultCharFilters.HtmlStrip}
                                }
                                
                               
                            }
                        }
                        
                    }
                },
            };

        }

        public void CreateTestData()
        {
            var jm = new PersonCity { Id = 1, FamilyName = "Moore", Info = "Muenich", Name = "John"};
            _context.AddUpdateDocument(jm, jm.Id);
            var jj = new PersonCity { Id = 2, FamilyName = "Jones", Info = "Münich", Name = "Johny" };
            _context.AddUpdateDocument(jj, jj.Id);
            var pm = new PersonCity { Id = 3, FamilyName = "Murphy", Info = "Munich", Name = "Paul" };
            _context.AddUpdateDocument(pm, pm.Id);
            var sm = new PersonCity { Id = 4, FamilyName = "McGurk", Info = "munich", Name = "Séan" };
            _context.AddUpdateDocument(sm, sm.Id);
            var sob = new PersonCity { Id = 5, FamilyName = "O'Brien", Info = "Not a much use, bit of a problem", Name = "Sean" };
            _context.AddUpdateDocument(sob, sob.Id);
            var tmc = new PersonCity { Id = 6, FamilyName = "McCauley", Info = "Couldn't a ask for anyone better", Name = "Tadhg" };
            _context.AddUpdateDocument(tmc, tmc.Id);
            var id7 = new PersonCity { Id = 7, FamilyName = "Martini", Info = "Köniz", Name = "Christian" };
            _context.AddUpdateDocument(id7, id7.Id);
            var id8 = new PersonCity { Id = 8, FamilyName = "Lee", Info = "Basel Stadt", Name = "Phil" };
            _context.AddUpdateDocument(id8, id8.Id);
            var id9 = new PersonCity { Id = 9, FamilyName = "Wil", Info = "Basel Stadt", Name = "Nicole" };
            _context.AddUpdateDocument(id9, id9.Id);
            var id10 = new PersonCity { Id = 10, FamilyName = "Mario", Info = "Basel in some small town", Name = "Tim" };
            _context.AddUpdateDocument(id10, id10.Id);
            var id11 = new PersonCity { Id = 11, FamilyName = "Martin", Info = "Biel", Name = "Scott" };
            _context.AddUpdateDocument(id11, id11.Id);
            var id12 = new PersonCity { Id = 12, FamilyName = "Newman", Info = "Lyss", Name = "Tim" };
            _context.AddUpdateDocument(id12, id12.Id);
            var id13 = new PersonCity { Id = 13, FamilyName = "Lamb", Info = "Thun", Name = "Carla" };
            _context.AddUpdateDocument(id13, id13.Id);
            var id14 = new PersonCity { Id = 14, FamilyName = "Goldi", Info = "Zug", Name = "Ida" };
            _context.AddUpdateDocument(id14, id14.Id);
            _context.SaveChanges();
        }

   
    }
}