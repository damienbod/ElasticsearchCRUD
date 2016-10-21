using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;

namespace SearchComponent
{
    public class PersonCityMappingDto
    {
        public long Id { get; set; }

        [ElasticsearchString(CopyToList = new[] { "autocomplete", "didYouMean" })]
        public string Name { get; set; }

        [ElasticsearchString(CopyToList = new[] { "autocomplete", "didYouMean" })]
        public string FamilyName { get; set; }

        [ElasticsearchString(CopyToList = new[] { "autocomplete", "didYouMean" })]
        public string Info { get; set; }

        [ElasticsearchString(Analyzer = "didYouMean")]
        public string did_you_mean { get; set; }

        [ElasticsearchString(Analyzer = "autocomplete")]
        public string autocomplete { get; set; }
    }
}
