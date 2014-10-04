using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
					MapCollectionOrArrayWithSimpleType(prop, entity);

					// TODO
					//if (object.IsElasticsearchDocumentType)
					//{
					//	// add to bulk insert as own index
					//}
				}
				else
				{
					if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String" && prop.PropertyType.FullName != "System.Decimal")
					{
						Writer.WritePropertyName(prop.Name.ToLower());
						Writer.WriteStartObject();
						// Do class mapping for nested type
						MapEntityValues(prop.GetValue(entity));
						Writer.WriteEndObject();
						

						// Add as document later
					}
					else
					{
						MapValue(prop.Name.ToLower(), prop.GetValue(entity));
					}
				}	
			}
		}

		// Nested
		// "tags" : ["elasticsearch", "wow"], (string array or int array)
		protected virtual void MapCollectionOrArrayWithSimpleType(PropertyInfo prop, object entity)
		{
			Writer.WritePropertyName(prop.Name.ToLower());
			Type type = prop.PropertyType;
			string json = null;
			if (type.HasElementType)
			{
				var ienumerable = (Array)prop.GetValue(entity);
				json = JsonConvert.SerializeObject(ienumerable);
			}
			else if (prop.PropertyType.IsGenericType)
			{
				var ienumerable = (IEnumerable)prop.GetValue(entity);
				json = JsonConvert.SerializeObject(ienumerable);
			}

			Writer.WriteRawValue(json);
		}

		// Nested
		//"lists" : [
		//	{
		//		"name" : "prog_list",
		//		"description" : "programming list"
		//	},
	
		public void AddWriter(JsonWriter writer)
		{
			Writer = writer;
		}

		protected void MapValue(string key, object valueObj)
		{
			Writer.WritePropertyName(key);
			Writer.WriteValue(valueObj);
		}

		protected bool IsPropertyACollection(PropertyInfo property)
		{
			if (property.PropertyType.FullName == "System.String" || property.PropertyType.FullName == "System.Decimal")
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
