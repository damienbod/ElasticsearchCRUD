using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
		// default type is lowercase for properties
		public virtual void MapEntityValues(Object entity, JsonWriter writer)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				if (IsPropertyACollection(prop))
				{
					// TODO test for object type or simple type
					MapCollectionOrArrayWithSimpleType(prop, entity, writer);

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
						writer.WritePropertyName(prop.Name.ToLower());
						writer.WriteStartObject();
						// Do class mapping for nested type
						MapEntityValues(prop.GetValue(entity), writer);
						writer.WriteEndObject();
						

						// Add as separate document later
					}
					else
					{
						MapValue(prop.Name.ToLower(), prop.GetValue(entity), writer);
					}
				}	
			}
		}

		// Nested
		// "tags" : ["elasticsearch", "wow"], (string array or int array)
		protected virtual void MapCollectionOrArrayWithSimpleType(PropertyInfo prop, object entity, JsonWriter writer)
		{
			writer.WritePropertyName(prop.Name.ToLower());
			Type type = prop.PropertyType;
			string json = null;
			if (type.HasElementType)
			{
				var ienumerable = (Array)prop.GetValue(entity);
				json = JsonConvert.SerializeObject(ienumerable);
				writer.WriteRawValue(json);					
			}
			else if (prop.PropertyType.IsGenericType)
			{
				// It is a collection
				var ienumerable = (IEnumerable)prop.GetValue(entity);
				if (ienumerable != null)
				{
					var sbCollection = new StringBuilder(); 
					foreach (var item in ienumerable)
					{
						var childEntityWriter = new JsonTextWriter(new StringWriter(sbCollection, CultureInfo.InvariantCulture)) { CloseOutput = true };
						var typeofArrayItem = item.GetType();
						if (typeofArrayItem.IsClass && typeofArrayItem.FullName != "System.String" && typeofArrayItem.FullName != "System.Decimal")
						{
							// collection of Objects
							childEntityWriter.WritePropertyName(prop.Name.ToLower());
							childEntityWriter.WriteStartObject();
							// Do class mapping for nested type
							MapEntityValues(item, childEntityWriter);
							childEntityWriter.WriteEndObject();

							// Add as separate document later
						}
						else
						{
							// collection of simple types, serialize all items in one go and break from the loop
							sbCollection.Append(JsonConvert.SerializeObject(ienumerable));
							
							break;
						}
					}

					writer.WriteRawValue(sbCollection.ToString());
				}
			}		
		}

		// Nested
		//"lists" : [
		//	{
		//		"name" : "prog_list",
		//		"description" : "programming list"
		//	},
	
		protected void MapValue(string key, object valueObj, JsonWriter writer)
		{
			writer.WritePropertyName(key);
			writer.WriteValue(valueObj);
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
