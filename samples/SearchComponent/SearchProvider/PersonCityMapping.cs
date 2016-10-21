using System;
using ElasticsearchCRUD;

namespace SearchComponent
{
    public class PersonCityMapping : ElasticsearchMapping
    {
        public override string GetIndexForType(Type type)
        {
            return "personcitys";
        }

        public override string GetDocumentType(Type type)
        {
            return "personcity";
        }
    }
}