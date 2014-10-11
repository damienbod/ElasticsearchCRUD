using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
		protected HashSet<string> SerializedTypes = new HashSet<string>();

		// default type is lowercase for properties
		public virtual void MapEntityValues(Object entity, JsonWriter writer, bool beginMappingTree = false)
		{
			if (beginMappingTree)
			{
				SerializedTypes = new HashSet<string>();
			}

			SerializedTypes.Add(GetDocumentType(entity.GetType()));

			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{				
				if (IsPropertyACollection(prop))
				{
					writer.WritePropertyName(prop.Name.ToLower());

					if (prop.GetValue(entity) != null)
					{
						var typeOfEntity = prop.GetValue(entity).GetType().GetGenericArguments();
						if (typeOfEntity.Length > 0)
						{
							if (!SerializedTypes.Contains(GetDocumentType(typeOfEntity[0])))
							{
								MapCollectionOrArray(prop, entity, writer);
							}
						}
						else
						{
							// Not a generic
							MapCollectionOrArray(prop, entity, writer);
						}
					}
				}
				else
				{
					if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String" && prop.PropertyType.FullName != "System.Decimal")
					{
						// This is a single object and not a reference to it's parent
						if (prop.GetValue(entity) != null && !SerializedTypes.Contains(GetDocumentType(prop.GetValue(entity).GetType())))
						{
							SerializedTypes.Add(GetDocumentType(prop.GetValue(entity).GetType()));

							writer.WritePropertyName(prop.Name.ToLower());
							writer.WriteStartObject();
							// Do class mapping for nested type
							MapEntityValues(prop.GetValue(entity), writer);
							writer.WriteEndObject();
						}

						// TODO Add as separate document later inn it's index
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
		// Nested
		//"lists" : [
		//	{
		//		"name" : "prog_list",
		//		"description" : "programming list"
		//	},	
		protected virtual void MapCollectionOrArray(PropertyInfo prop, object entity, JsonWriter writer)
		{
			Type type = prop.PropertyType;
			
			if (type.HasElementType)
			{
				// It is a collection
				var ienumerable = (Array)prop.GetValue(entity);
				MapIEnumerableEntities(writer, ienumerable);				
			}
			else if (prop.PropertyType.IsGenericType)
			{
				// It is a collection
				var ienumerable = (IEnumerable)prop.GetValue(entity);
				MapIEnumerableEntities(writer, ienumerable);
			}
		}

		private void MapIEnumerableEntities(JsonWriter writer, IEnumerable ienumerable)
		{
			string json = null;
			bool isSimpleArrayOrCollection = true;
			bool doProccessingIfTheIEnumerableHasAtLeastOneItem = false;
			if (ienumerable != null)
			{
				var sbCollection = new StringBuilder();
				sbCollection.Append("[");
				foreach (var item in ienumerable)
				{

					doProccessingIfTheIEnumerableHasAtLeastOneItem = true;
					var childEntityWriter = new JsonTextWriter(new StringWriter(sbCollection, CultureInfo.InvariantCulture))
					{
						CloseOutput = true
					};
					var typeofArrayItem = item.GetType();
					if (typeofArrayItem.IsClass && typeofArrayItem.FullName != "System.String" &&
						typeofArrayItem.FullName != "System.Decimal")
					{
						isSimpleArrayOrCollection = false;
						// collection of Objects
						childEntityWriter.WriteStartObject();
						// Do class mapping for nested type
						MapEntityValues(item, childEntityWriter);
						childEntityWriter.WriteEndObject();

						// Add as separate document later
					}
					else
					{
						// collection of simple types, serialize all items in one go and break from the loop
						json = JsonConvert.SerializeObject(ienumerable);

						break;
					}
					sbCollection.Append(",");
				}

				if (doProccessingIfTheIEnumerableHasAtLeastOneItem)
				{
					if (isSimpleArrayOrCollection)
					{
						writer.WriteRawValue(json);
					}
					else
					{
						sbCollection.Remove(sbCollection.Length - 1, 1);
						sbCollection.Append("]");
						writer.WriteRawValue(sbCollection.ToString());
					}
				}
					
			}
		}

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

		public virtual object ParseEntity(JToken source, Type type)
		{
			return JsonConvert.DeserializeObject(source.ToString(), type);
		}

		public virtual string GetDocumentType(Type type)
		{
			// Adding support for EF types
			if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
			{
				type = type.BaseType;
			}
			return type.Name.ToLower();
		}

		// pluralize the default type
		public virtual string GetIndexForType(Type type)
		{
			// Adding support for EF types
			if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
			{
				type = type.BaseType;
			}
			return type.Name.ToLower() + "s";
		}
	}

}
