using System;
using ElasticsearchCRUD;

namespace SearchComponent
{
    public class PersonCityMapping : ElasticsearchMapping
    {
        public override string GetIndexForType(Type type)
        {
            return "personcity";
        }

        public override string GetDocumentType(Type type)
        {
            return "personcitys";
        }
    }
}