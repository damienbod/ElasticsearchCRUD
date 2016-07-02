using System;
using System.Reflection;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
    using System.Collections.Generic;
    using System.Linq;

    public class Fields
    {
        /// <summary>
        /// You can define all the Elasticsearch properties here
        /// </summary>
        public Type FieldClass { get; set; }

        public void AddFieldData(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
        {
            elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("fields");
            elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

            var propertyInfo = FieldClass.GetProperties();
            foreach (var property in propertyInfo)
            {
//#if NET46 || NET451 || NET452
//                if (Attribute.IsDefined(property, typeof(ElasticsearchCoreTypes)))
//                {
//                    var propertyName = property.Name.ToLower(); 
//                    object[] attrs = property.GetCustomAttributes(typeof(ElasticsearchCoreTypes), true);

//                    if ((attrs[0] as ElasticsearchCoreTypes) != null)
//                    {
//                        elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(propertyName);
//                        elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue((attrs[0] as ElasticsearchCoreTypes).JsonString());
//                    }
//                } 
//#else          
                if (property.GetCustomAttribute(typeof(ElasticsearchCoreTypes)) != null)
                {
                    var propertyName = property.Name.ToLower();
                                 
                    IEnumerable<Attribute> attrs = property.GetCustomAttributes(typeof(ElasticsearchCoreTypes), true);

                    if ((attrs.FirstOrDefault() as ElasticsearchCoreTypes) != null)
                    {
                        elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(propertyName);
                        elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue((attrs.FirstOrDefault() as ElasticsearchCoreTypes).JsonString());
                    }
                }
//#endif

            }

            elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    
}