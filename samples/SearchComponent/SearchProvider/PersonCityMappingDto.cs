using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;

namespace SearchComponent
{
    public class PersonCityMappingDto
    {
        public long Id { get; set; }

        [ElasticsearchString(CopyToList = new[] { "autocomplete", "did_you_mean" })]
        public string Name { get; set; }

        [ElasticsearchString(CopyToList = new[] { "autocomplete", "did_you_mean" })]
        public string FamilyName { get; set; }

        [ElasticsearchString(CopyToList = new[] { "autocomplete", "did_you_mean" })]
        public string Info { get; set; }

        [ElasticsearchString(Analyzer = "didyoumean")]
        public string did_you_mean { get; set; }

        [ElasticsearchString(Analyzer = "autocomplete")]
        public string autocomplete { get; set; }
    }
}
