using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	public static class IEnumerableExtensions
	{
		public static void ForEachExceptTheLast<T>(
			this IEnumerable<T> source,
			Action<T> usualAction,
			Action<T> lastAction
		)
		{
			var e = source.GetEnumerator();
			T penultimate;
			T last;
			if (e.MoveNext())
			{
				last = e.Current;
				while (e.MoveNext())
				{
					penultimate = last;
					last = e.Current;
					usualAction(penultimate);
				}
				lastAction(last);
			}
		}
	} 

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
					MapSimpleArrayValue(prop, entity);

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
		protected void MapSimpleArrayValue(PropertyInfo prop, object entity)
		{
			var localArray = new StringBuilder();
			localArray.Append(",\"" + prop.Name.ToLower() + "\" : ");
			localArray.Append("[");
			Type type = prop.PropertyType;
			if (type.HasElementType)
			{
				var ienumerable = (Array)prop.GetValue(entity);

				foreach (var item in ienumerable)
				{
					Writer.WriteValue(item);
				}
			}

			//// prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
			if (prop.PropertyType.IsGenericType)
			{
				var ienumerable = (IEnumerable)prop.GetValue(entity);
		
				
				foreach (var item in ienumerable)
				{
					localArray.Append("\"" + item + "\",");
				}
			}
			// remove the last comma
			localArray.Remove(localArray.Length - 1, 1);

			localArray.Append("]");
			Writer.WriteRaw(localArray.ToString());
		}

		// Nested
		//"lists" : [
		//	{
		//		"name" : "prog_list",
		//		"description" : "programming list"
		//	},
		protected void MapObjectArrayValue(PropertyInfo prop, object entity)
		{
			//object[] ienumerable = (object[])prop.GetValue(entity);
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
