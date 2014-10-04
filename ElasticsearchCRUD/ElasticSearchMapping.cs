using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Default mapping for your Entity. You can implement this clas to implement your specific mapping if required
	/// Everything is lowercase and the index is pluralized
	/// </summary>
	public class ElasticSearchMapping
	{
		protected JsonWriter Writer;

		// default type is lowercase for properties
		public virtual void MapEntityValues(Object entity)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				if (IsPropertyACollection(prop))
				{
					// TODO test for object type or simple type
					MapSimpleArrayValue(prop);

					// TODO
					//if (object.IsElasticsearchDocumentType)
					//{
					//	// add to bulk insert as own index
					//}
					//else
					//{
					//	MapObjectArrayValue(prop);
					//}
				}
				else
				{
					MapValue(prop.Name.ToLower(), prop.GetValue(entity));
				}	
			}
		}

		// Nested
		// "tags" : ["elasticsearch", "wow"], (string array or int array)
		protected void MapSimpleArrayValue(PropertyInfo prop)
		{
			Writer.WriteEndObject();
			//Writer.WritePropertyName("data:");
			Writer.WriteStartArray();

			for (int t = 0; t < 1; t++)
			{
				
			}

			Writer.WriteEndArray();
			Writer.WriteStartObject();
		}

		// Nested
		//"lists" : [
		//	{
		//		"name" : "prog_list",
		//		"description" : "programming list"
		//	},
		protected void MapObjectArrayValue(PropertyInfo prop)
		{
		}
		
		public void AddWriter(JsonWriter writer)
		{
			Writer = writer;
		}

		protected void MapValue(string key, object valueObj)
		{
			Writer.WritePropertyName(key);
			Writer.WriteValue(valueObj);
		}

		public bool IsPropertyACollection(PropertyInfo property)
		{
			if (property.PropertyType.FullName == "System.String")
			{
				return false;
			}
			return property.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) != null;
		} 

		public virtual object ParseEntity(Newtonsoft.Json.Linq.JToken source, Type type)
		{
			return JsonConvert.DeserializeObject(source.ToString(), type);
		}

		public virtual string GetDocumentType(Type type)
		{
			return type.Name.ToLower();
		}

		// pluralize the default type
		public virtual string GetIndexForType(Type type)
		{
			return type.Name.ToLower() + "s";
		}
	}

}
