using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchGermanAnalyzer
{
    using ElasticsearchCRUD;
    using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
    using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
    using ElasticsearchCRUD.ContextSearch.SearchModel;
    using ElasticsearchCRUD.Model;
    using ElasticsearchCRUD.Model.SearchModel;
    using ElasticsearchCRUD.Model.SearchModel.Queries;
    using ElasticsearchCRUD.Tracing;

    public class ElasticsearchGermanSearchProvider
    {
        public ElasticsearchGermanSearchProvider()
        {
            _context = new ElasticsearchContext(ConnectionString, new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver))
            {
                TraceProvider = new ConsoleTraceProvider()
            };
        }
        private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
        private const string ConnectionString = "http://localhost:9200";
        private readonly ElasticsearchContext _context;

        public void CreateIndex(IndexDefinition indexDefinition)
        {
            _context.IndexCreate<GermanData>(indexDefinition);
        }

        public void CreateSomeMembers()
        {
            var jm = new GermanData { Id = 1, FamilyName = "Moore", Info = "Muenich", Name = "John" };
            _context.AddUpdateDocument(jm, jm.Id);
            var jj = new GermanData { Id = 2, FamilyName = "Jones", Info = "Münich", Name = "Johny" };
            _context.AddUpdateDocument(jj, jj.Id);
            var pm = new GermanData { Id = 3, FamilyName = "Murphy", Info = "Munich", Name = "Paul" };
            _context.AddUpdateDocument(pm, pm.Id);
            var sm = new GermanData { Id = 4, FamilyName = "McGurk", Info = "munich", Name = "Séan" };
            _context.AddUpdateDocument(sm, sm.Id);
            var sob = new GermanData { Id = 5, FamilyName = "O'Brien", Info = "Not a much use, bit of a problem", Name = "Sean" };
            _context.AddUpdateDocument(sob, sob.Id);
            var tmc = new GermanData { Id = 6, FamilyName = "McCauley", Info = "Couldn't a ask for anyone better", Name = "Tadhg" };
            _context.AddUpdateDocument(tmc, tmc.Id);

            _context.SaveChanges();
        }

        //{
        //  "query": {
        //		"match": {"name": "sean"}
        //	 }
        //  }
        //}
        public SearchResult<GermanData> Search(string name)
        {
            var search = new Search()
            {
                Query = new Query(new MatchQuery("info", name))
            };
            return _context.Search<GermanData>(search).PayloadResult;
        }

    }

    public class GermanData
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string FamilyName { get; set; }

        [ElasticsearchString(Fields = typeof(FieldDataDefinition), Analyzer = LanguageAnalyzers.German)]
        public string Info { get; set; }
    }


    public class FieldDataDefinition
    {
        [ElasticsearchString(Index = StringIndex.not_analyzed)]
        public string Raw { get; set; }
    }
}
