using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ElasticsearchCRUD.Tracing;
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
		public ITraceProvider TraceProvider = new NullTraceProvider();
		public bool IncludeChildObjectsInDocument { get; set; }
		public bool ProcessChildDocumentsAsSeparateChildIndex { get; set; }

		public List<EntityContextInfo> ChildIndexEntities = new List<EntityContextInfo>();
 
		// default type is lowercase for properties
		public virtual void MapEntityValues(Object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool beginMappingTree = false)
		{
			try
			{
				BeginNewEntityToDocumentMapping(entity, beginMappingTree);

				SerializedTypes.Add(GetDocumentType(entity.GetType()));
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: SerializedTypes new Type added: {0}", GetDocumentType(entity.GetType()));
				var propertyInfo = entity.GetType().GetProperties();
				foreach (var prop in propertyInfo)
				{
					if (!Attribute.IsDefined(prop, typeof (JsonIgnoreAttribute)))
					{
						if (IsPropertyACollection(prop))
						{
							ProcessArrayOrCollection(entity, elasticsearchCrudJsonWriter, prop);
						}
						else
						{
							if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String" && prop.PropertyType.FullName != "System.Decimal")
							{
								ProcessSingleObject(entity, elasticsearchCrudJsonWriter, prop);
							}
							else
							{
								TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: Property is a simple Type: {0}", prop.Name.ToLower());
								MapValue(prop.Name.ToLower(), prop.GetValue(entity), elasticsearchCrudJsonWriter.JsonWriter);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				TraceProvider.Trace(TraceEventType.Critical, ex, "ElasticSearchMapping: Property is a simple Type: {0}", elasticsearchCrudJsonWriter.GetJsonString());
				throw;
			}
		}

		private void BeginNewEntityToDocumentMapping(object entity, bool beginMappingTree)
		{
			if (beginMappingTree)
			{
				SerializedTypes = new HashSet<string>();
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: Serialize BEGIN for Type: {0}", entity.GetType());
			}
		}

		private void ProcessSingleObject(object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: Property is an Object: {0}", prop.ToString());
			// This is a single object and not a reference to it's parent
			if (prop.GetValue(entity) != null && !SerializedTypes.Contains(GetDocumentType(prop.GetValue(entity).GetType())) && IncludeChildObjectsInDocument)
			{
				// TODO Add as separate document later in it's index, Release V1.0.8
				ProcessSingleObjectAsNestedObject(entity, elasticsearchCrudJsonWriter, prop);
			}
		}

		private void ProcessArrayOrCollection(object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: IsPropertyACollection: {0}", prop.Name.ToLower());
			if (prop.GetValue(entity) != null && IncludeChildObjectsInDocument)
			{
				// TODO Add as separate child documents later in it's index Release V1.0.8

				ProcessArrayOrCollectionAsNestedObject(entity, elasticsearchCrudJsonWriter, prop);
			}
		}

		private void ProcessSingleObjectAsNestedObject(object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			// Do class mapping for nested type
			MapEntityValues(prop.GetValue(entity), elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void ProcessArrayOrCollectionAsNestedObject(object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: BEGIN ARRAY or COLLECTION: {0} {1}", prop.Name.ToLower(), elasticsearchCrudJsonWriter.JsonWriter.Path);
			var typeOfEntity = prop.GetValue(entity).GetType().GetGenericArguments();
			if (typeOfEntity.Length > 0)
			{
				if (!SerializedTypes.Contains(GetDocumentType(typeOfEntity[0])))
				{
					TraceProvider.Trace(TraceEventType.Verbose,
						"ElasticSearchMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
					TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: SerializedTypes new Type added: {0}",
						GetDocumentType(typeOfEntity[0]));
					MapCollectionOrArray(prop, entity, elasticsearchCrudJsonWriter);
				}
				else
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("null");
				}
			}
			else
			{
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: BEGIN ARRAY or COLLECTION NOT A GENERIC: {0}",
					prop.Name.ToLower());
				// Not a generic
				MapCollectionOrArray(prop, entity, elasticsearchCrudJsonWriter);
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
		protected virtual void MapCollectionOrArray(PropertyInfo prop, object entity, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			Type type = prop.PropertyType;
			
			if (type.HasElementType)
			{
				// It is a collection
				var ienumerable = (Array)prop.GetValue(entity);
				MapIEnumerableEntities(elasticsearchCrudJsonWriter, ienumerable);				
			}
			else if (prop.PropertyType.IsGenericType)
			{
				// It is a collection
				var ienumerable = (IEnumerable)prop.GetValue(entity);
				MapIEnumerableEntities(elasticsearchCrudJsonWriter, ienumerable);
			}
		}

		private void MapIEnumerableEntities(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, IEnumerable ienumerable)
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

					var childElasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter(sbCollection);
					elasticsearchCrudJsonWriter.ElasticsearchCrudJsonWriterChildItem = childElasticsearchCrudJsonWriter;

					var typeofArrayItem = item.GetType();
					if (typeofArrayItem.IsClass && typeofArrayItem.FullName != "System.String" &&
						typeofArrayItem.FullName != "System.Decimal")
					{
						isSimpleArrayOrCollection = false;
						// collection of Objects
						childElasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
						// Do class mapping for nested type
						MapEntityValues(item, childElasticsearchCrudJsonWriter);
						childElasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

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

				if (isSimpleArrayOrCollection && doProccessingIfTheIEnumerableHasAtLeastOneItem)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue(json);
				}
				else
				{
					if (doProccessingIfTheIEnumerableHasAtLeastOneItem)

					{
						sbCollection.Remove(sbCollection.Length - 1, 1);
					}

					sbCollection.Append("]");
					elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue(sbCollection.ToString());
				}
			}
			else
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("");
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
