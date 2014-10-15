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
		public bool SaveChildObjectsAsWellAsParent { get; set; }
		public bool ProcessChildDocumentsAsSeparateChildIndex { get; set; }

		public List<EntityContextInfo> ChildIndexEntities = new List<EntityContextInfo>();

		// default type is lowercase for properties
		public virtual void MapEntityValues(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool beginMappingTree = false)
		{
			try
			{
				BeginNewEntityToDocumentMapping(entityInfo, beginMappingTree);

				SerializedTypes.Add(GetDocumentType(entityInfo.Entity.GetType()));
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: SerializedTypes new Type added: {0}", GetDocumentType(entityInfo.Entity.GetType()));
				var propertyInfo = entityInfo.Entity.GetType().GetProperties();
				foreach (var prop in propertyInfo)
				{
					if (!Attribute.IsDefined(prop, typeof (JsonIgnoreAttribute)))
					{
						if (IsPropertyACollection(prop))
						{
							ProcessArrayOrCollection(entityInfo, elasticsearchCrudJsonWriter, prop);
						}
						else
						{
							if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String" && prop.PropertyType.FullName != "System.Decimal")
							{
								ProcessSingleObject(entityInfo, elasticsearchCrudJsonWriter, prop);
							}
							else
							{
								TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: Property is a simple Type: {0}", prop.Name.ToLower());
								MapValue(prop.Name.ToLower(), prop.GetValue(entityInfo.Entity), elasticsearchCrudJsonWriter.JsonWriter);
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

		private void BeginNewEntityToDocumentMapping(EntityContextInfo entityInfo, bool beginMappingTree)
		{
			if (beginMappingTree)
			{
				SerializedTypes = new HashSet<string>();
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: Serialize BEGIN for Type: {0}", entityInfo.Entity.GetType());
			}
		}

		private void ProcessSingleObject(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: Property is an Object: {0}", prop.ToString());
			// This is a single object and not a reference to it's parent
			if (prop.GetValue(entityInfo.Entity) != null && !SerializedTypes.Contains(GetDocumentType(prop.GetValue(entityInfo.Entity).GetType())) && SaveChildObjectsAsWellAsParent)
			{
				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					ProcessSingleObjectAsChildDocument(entityInfo, elasticsearchCrudJsonWriter, prop);
				}
				else
				{	
					ProcessSingleObjectAsNestedObject(entityInfo, elasticsearchCrudJsonWriter, prop);
				}
			}
		}

		private void ProcessArrayOrCollection(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: IsPropertyACollection: {0}", prop.Name.ToLower());
			if (prop.GetValue(entityInfo.Entity) != null && SaveChildObjectsAsWellAsParent)
			{
				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					ProcessArrayOrCollectionAsChildDocument(entityInfo, elasticsearchCrudJsonWriter, prop);
				}
				else
				{
					ProcessArrayOrCollectionAsNestedObject(entityInfo, elasticsearchCrudJsonWriter, prop);
				}
			}
		}

		private void ProcessSingleObjectAsNestedObject(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			// Do class mapping for nested type

			// TODO add child Id
			var entity = prop.GetValue(entityInfo.Entity);
			var child = new EntityContextInfo { Entity = entity, ParentId = entityInfo.Id, EntityType = entity.GetType(), DeleteEntity = entityInfo.DeleteEntity};
			MapEntityValues(child, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void ProcessSingleObjectAsChildDocument(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			// TODO add child Id
			var entity = prop.GetValue(entityInfo.Entity);
			var child = new EntityContextInfo { Entity = entity, ParentId = entityInfo.Id, EntityType = entity.GetType(), DeleteEntity = entityInfo.DeleteEntity };
			MapEntityValues(child, elasticsearchCrudJsonWriter);
		}

		private void ProcessArrayOrCollectionAsNestedObject(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: BEGIN ARRAY or COLLECTION: {0} {1}", prop.Name.ToLower(), elasticsearchCrudJsonWriter.JsonWriter.Path);
			var typeOfEntity = prop.GetValue(entityInfo.Entity).GetType().GetGenericArguments();
			if (typeOfEntity.Length > 0)
			{
				if (!SerializedTypes.Contains(GetDocumentType(typeOfEntity[0])))
				{
					TraceProvider.Trace(TraceEventType.Verbose,
						"ElasticSearchMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
					TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: SerializedTypes new Type added: {0}",
						GetDocumentType(typeOfEntity[0]));
					MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter);
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
				MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter);
			}
		}

		private void ProcessArrayOrCollectionAsChildDocument(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: BEGIN ARRAY or COLLECTION: {0} {1}", prop.Name.ToLower(), elasticsearchCrudJsonWriter.JsonWriter.Path);
			var typeOfEntity = prop.GetValue(entityInfo.Entity).GetType().GetGenericArguments();
			if (typeOfEntity.Length > 0)
			{
				if (!SerializedTypes.Contains(GetDocumentType(typeOfEntity[0])))
				{
					TraceProvider.Trace(TraceEventType.Verbose,
						"ElasticSearchMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
					TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: SerializedTypes new Type added: {0}",
						GetDocumentType(typeOfEntity[0]));

					MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter);
				}
			}
			else
			{
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticSearchMapping: BEGIN ARRAY or COLLECTION NOT A GENERIC: {0}",
					prop.Name.ToLower());
				// Not a generic
				MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter);
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
		protected virtual void MapCollectionOrArray(PropertyInfo prop, EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			Type type = prop.PropertyType;
			
			if (type.HasElementType)
			{
				// It is a collection
				var ienumerable = (Array)prop.GetValue(entityInfo.Entity);
				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					// TODO process as child doc
				}
				else
				{
					MapIEnumerableEntities(elasticsearchCrudJsonWriter, ienumerable, entityInfo);	
				}
							
			}
			else if (prop.PropertyType.IsGenericType)
			{
				// It is a collection
				var ienumerable = (IEnumerable)prop.GetValue(entityInfo.Entity);
				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					// TODO process as child doc
				}
				else
				{
					MapIEnumerableEntities(elasticsearchCrudJsonWriter, ienumerable, entityInfo);
				}
			}
		}

		private void MapIEnumerableEntities(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, IEnumerable ienumerable, EntityContextInfo parentEntityInfo)
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
						// TODO add child Id
						var child = new EntityContextInfo { Entity = item, ParentId = parentEntityInfo.Id, EntityType = item.GetType(), DeleteEntity = parentEntityInfo.DeleteEntity };
						MapEntityValues(child, childElasticsearchCrudJsonWriter);
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
