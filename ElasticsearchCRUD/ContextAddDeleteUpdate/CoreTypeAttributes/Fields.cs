using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
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
				if (Attribute.IsDefined(property, typeof(ElasticsearchCoreTypes)))
				{
					var propertyName = property.Name.ToLower(); 
					object[] attrs = property.GetCustomAttributes(typeof(ElasticsearchCoreTypes), true);

					if ((attrs[0] as ElasticsearchCoreTypes) != null)
					{
						elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(propertyName);
						elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue((attrs[0] as ElasticsearchCoreTypes).JsonString());
					}
				}
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	
}