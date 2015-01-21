using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Default mapping for your Entity. You can implement this clas to implement your specific mapping if required
	/// Everything is lowercase and the index is pluralized
	/// </summary>
	public class ElasticsearchMapping
	{
		protected HashSet<string> SerializedTypes = new HashSet<string>();
		public ITraceProvider TraceProvider = new NullTraceProvider();
		public bool SaveChildObjectsAsWellAsParent { get; set; }
		public bool ProcessChildDocumentsAsSeparateChildIndex { get; set; }

		public List<EntityContextInfo> ChildIndexEntities = new List<EntityContextInfo>();

		/// <summary>
		/// Ovveride this if your default mapping needs to be changed.
		/// default type is lowercase for properties, indes pluralized and type to lower
		/// </summary>
		/// <param name="entityInfo">Information about the entity</param>
		/// <param name="elasticsearchCrudJsonWriter">Serializer with added tracing</param>
		/// <param name="beginMappingTree">begin new mapping tree</param>
		/// <param name="createPropertyMappings">This tells the serializer to create a Json property mapping from the entity and not the document itself</param>
		public virtual void MapEntityValues(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool beginMappingTree = false, bool createPropertyMappings = false)
		{
			try
			{
				BeginNewEntityToDocumentMapping(entityInfo, beginMappingTree);

				TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: SerializedTypes new Type added: {0}", GetDocumentType(entityInfo.Document.GetType()));
				var propertyInfo = entityInfo.Document.GetType().GetProperties();
				foreach (var prop in propertyInfo)
				{
					if (!Attribute.IsDefined(prop, typeof (JsonIgnoreAttribute)))
					{
						if (Attribute.IsDefined(prop, typeof (ElasticsearchGeoTypeAttribute))) 
						{
							var obj = prop.Name.ToLower();
							// process GeoTypes
							if (createPropertyMappings)
							{
								object[] attrs = prop.GetCustomAttributes(typeof (ElasticsearchCoreTypes), true);

								if ((attrs[0] as ElasticsearchCoreTypes) != null)
								{
									elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(obj);
									elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue((attrs[0] as ElasticsearchCoreTypes).JsonString());
								}
							}
							else
							{
								var data = prop.GetValue(entityInfo.Document) as IGeoType;
								elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(obj);							
								data.WriteJson(elasticsearchCrudJsonWriter);
								// Write data
							}
						}
						else if (IsPropertyACollection(prop))
						{
							ProcessArrayOrCollection(entityInfo, elasticsearchCrudJsonWriter, prop, createPropertyMappings);
						}
						else
						{
							if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String" && prop.PropertyType.FullName != "System.Decimal")
							{
								ProcessSingleObject(entityInfo, elasticsearchCrudJsonWriter, prop, createPropertyMappings);
							}
							else
							{
								if (!ProcessChildDocumentsAsSeparateChildIndex || ProcessChildDocumentsAsSeparateChildIndex && beginMappingTree)
								{
									TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: Property is a simple Type: {0}, {1}", prop.Name.ToLower(), prop.PropertyType.FullName);

									if (createPropertyMappings)
									{
										var obj = prop.Name.ToLower();
										if (Attribute.IsDefined(prop, typeof (ElasticsearchCoreTypes)))
										{									
											object[] attrs = prop.GetCustomAttributes(typeof (ElasticsearchCoreTypes), true);

											if ((attrs[0] as ElasticsearchCoreTypes) != null)
											{
												elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(obj);
												elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue((attrs[0] as ElasticsearchCoreTypes).JsonString());
											}
											
										}
										else
										{
											// no elasticsearch property defined
											elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(obj);
											if (prop.PropertyType.FullName == "System.DateTime" || prop.PropertyType.FullName == "System.DateTimeOffset")
											{
												elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("{ \"type\" : \"date\", \"format\": \"dateOptionalTime\"}");
											}
											else
											{
												elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("{ \"type\" : \"" + GetElasticsearchType(prop.PropertyType) + "\" }");
											}

										}
									}
									else
									{
										MapValue(prop.Name.ToLower(), prop.GetValue(entityInfo.Document), elasticsearchCrudJsonWriter.JsonWriter);
									}

								}
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				TraceProvider.Trace(TraceEventType.Critical, ex, "ElasticsearchMapping: Property is a simple Type: {0}", elasticsearchCrudJsonWriter.GetJsonString());
				throw;
			}
		}

		private void BeginNewEntityToDocumentMapping(EntityContextInfo entityInfo, bool beginMappingTree)
		{
			if (beginMappingTree)
			{
				SerializedTypes = new HashSet<string>();
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: Serialize BEGIN for Type: {0}", entityInfo.Document.GetType());
			}
		}

		private void ProcessSingleObject(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: Property is an Object: {0}", prop.ToString());
			// This is a single object and not a reference to it's parent

			if (createPropertyMappings && prop.GetValue(entityInfo.Document) == null)
			{
				prop.SetValue(entityInfo.Document,Activator.CreateInstance(prop.PropertyType));
			}
			if (prop.GetValue(entityInfo.Document) != null  && SaveChildObjectsAsWellAsParent)
			{
				var child = GetDocumentType(prop.GetValue(entityInfo.Document).GetType());
				var parent = GetDocumentType(entityInfo.EntityType);
				if (!SerializedTypes.Contains(child + parent))
				{
					SerializedTypes.Add(parent + child);
					if (ProcessChildDocumentsAsSeparateChildIndex)
					{
						ProcessSingleObjectAsChildDocument(entityInfo, elasticsearchCrudJsonWriter, prop, createPropertyMappings);
					}
					else
					{
						ProcessSingleObjectAsNestedObject(entityInfo, elasticsearchCrudJsonWriter, prop, createPropertyMappings);
					}
				}
			}
		}

		private void ProcessArrayOrCollection(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: IsPropertyACollection: {0}", prop.Name.ToLower());

			if (createPropertyMappings && prop.GetValue(entityInfo.Document) == null)
			{
				if (prop.PropertyType.IsArray)
				{
					prop.SetValue(entityInfo.Document, Array.CreateInstance(prop.PropertyType.GetElementType(), 0));
				}
				else
				{
					prop.SetValue(entityInfo.Document, Activator.CreateInstance(prop.PropertyType));
				}	
			}

			if (prop.GetValue(entityInfo.Document) != null && SaveChildObjectsAsWellAsParent)
			{
				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					ProcessArrayOrCollectionAsChildDocument(entityInfo, elasticsearchCrudJsonWriter, prop, createPropertyMappings);
				}
				else
				{
					ProcessArrayOrCollectionAsNestedObject(entityInfo, elasticsearchCrudJsonWriter, prop, createPropertyMappings);
				}
			}
		}

		private void ProcessSingleObjectAsNestedObject(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (createPropertyMappings)
			{
				// "properties": {
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("properties");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			}
			// Do class mapping for nested type
			var entity = prop.GetValue(entityInfo.Document);
			var routingDefinition = new RoutingDefinition {ParentId = entityInfo.Id};
			var child = new EntityContextInfo { Document = entity, RoutingDefinition = routingDefinition, EntityType = entity.GetType(), DeleteDocument = entityInfo.DeleteDocument };

			MapEntityValues(child, elasticsearchCrudJsonWriter, false, createPropertyMappings);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (createPropertyMappings)
			{
				
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		private void ProcessSingleObjectAsChildDocument(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
		{
			var entity = prop.GetValue(entityInfo.Document);
			CreateChildEntityForDocumentIndex(entityInfo, elasticsearchCrudJsonWriter, entity, createPropertyMappings);
		}

		private void CreateChildEntityForDocumentIndex(EntityContextInfo parentEntityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, object entity, bool createPropertyMappings)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var property in propertyInfo)
			{
				if (Attribute.IsDefined(property, typeof(KeyAttribute)) || Attribute.IsDefined(property, typeof(ElasticsearchIdAttribute)))
				{
					var obj = property.GetValue(entity);

					if (obj == null && createPropertyMappings)
					{
						obj = "0";
					}

					RoutingDefinition routingDefinition;
					if (parentEntityInfo.RoutingDefinition.RoutingId != null)
					{
						// child of a child or lower...
						routingDefinition = new RoutingDefinition { ParentId = parentEntityInfo.Id, RoutingId = parentEntityInfo.RoutingDefinition.RoutingId };
					}
					else
					{
						// This is a direct child
						routingDefinition = new RoutingDefinition { ParentId = parentEntityInfo.Id, RoutingId = parentEntityInfo.Id };
					}
					
					var child = new EntityContextInfo
					{
						Document = entity,
						RoutingDefinition = routingDefinition,
						EntityType = GetEntityDocumentType(entity.GetType()),
						ParentEntityType = GetEntityDocumentType(parentEntityInfo.EntityType),
						DeleteDocument = parentEntityInfo.DeleteDocument,
						Id = obj.ToString() 
					};
					ChildIndexEntities.Add(child);
					MapEntityValues(child, elasticsearchCrudJsonWriter, false, createPropertyMappings);
				
					return;
				}
			}

			throw new ElasticsearchCrudException("No Key found for child object: " + parentEntityInfo.Document.GetType());
		}

		private void ProcessArrayOrCollectionAsNestedObject(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(prop.Name.ToLower());
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: BEGIN ARRAY or COLLECTION: {0} {1}", prop.Name.ToLower(), elasticsearchCrudJsonWriter.JsonWriter.Path);
			var typeOfEntity = prop.GetValue(entityInfo.Document).GetType().GetGenericArguments();
			if (typeOfEntity.Length > 0)
			{
				var child = GetDocumentType(typeOfEntity[0]);
				var parent = GetDocumentType(entityInfo.EntityType);

				if (!SerializedTypes.Contains(child + parent))
				{
					SerializedTypes.Add(parent + child);
					TraceProvider.Trace(TraceEventType.Verbose,
						"ElasticsearchMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
					TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: SerializedTypes new Type added: {0}",
						GetDocumentType(typeOfEntity[0]));
					MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter, createPropertyMappings);
				}
				else
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("null");
				}
			}
			else
			{
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: BEGIN ARRAY or COLLECTION NOT A GENERIC: {0}",
					prop.Name.ToLower());
				// Not a generic
				MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter, createPropertyMappings);
			}
		}

		private void ProcessArrayOrCollectionAsChildDocument(EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, PropertyInfo prop, bool createPropertyMappings)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: BEGIN ARRAY or COLLECTION: {0} {1}", prop.Name.ToLower(), elasticsearchCrudJsonWriter.JsonWriter.Path);
			var typeOfEntity = prop.GetValue(entityInfo.Document).GetType().GetGenericArguments();
			if (typeOfEntity.Length > 0)
			{
				var child = GetDocumentType(typeOfEntity[0]);
				var parent = GetDocumentType(entityInfo.EntityType);

				if (!SerializedTypes.Contains(child + parent))
				{
					SerializedTypes.Add(parent + child);
					TraceProvider.Trace(TraceEventType.Verbose,
						"ElasticsearchMapping: SerializedTypes type ok, BEGIN ARRAY or COLLECTION: {0}", typeOfEntity[0]);
					TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: SerializedTypes new Type added: {0}",
						GetDocumentType(typeOfEntity[0]));

					MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter, createPropertyMappings);
				}
			}
			else
			{
				TraceProvider.Trace(TraceEventType.Verbose, "ElasticsearchMapping: BEGIN ARRAY or COLLECTION NOT A GENERIC: {0}",
					prop.Name.ToLower());
				// Not a generic
				MapCollectionOrArray(prop, entityInfo, elasticsearchCrudJsonWriter, createPropertyMappings);
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
		protected virtual void MapCollectionOrArray(PropertyInfo prop, EntityContextInfo entityInfo, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool createPropertyMappings)
		{
			Type type = prop.PropertyType;
			
			if (type.HasElementType)
			{
				// It is a collection
				var ienumerable = (Array)prop.GetValue(entityInfo.Document);
				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					MapIEnumerableEntitiesForChildIndexes(elasticsearchCrudJsonWriter, ienumerable, entityInfo, prop, createPropertyMappings);
				}
				else
				{
					if (createPropertyMappings)
					{
						MapIEnumerableEntitiesForMapping(elasticsearchCrudJsonWriter, entityInfo, prop,true);	
					}
					else
					{
						MapIEnumerableEntities(elasticsearchCrudJsonWriter, ienumerable, entityInfo, false);	
					}
					
				}
							
			}
			else if (prop.PropertyType.IsGenericType)
			{
				// It is a collection
				var ienumerable = (IEnumerable)prop.GetValue(entityInfo.Document);

				if (ProcessChildDocumentsAsSeparateChildIndex)
				{
					MapIEnumerableEntitiesForChildIndexes(elasticsearchCrudJsonWriter, ienumerable, entityInfo, prop, createPropertyMappings);
				}
				else
				{
					if (createPropertyMappings)
					{
						MapIEnumerableEntitiesForMapping(elasticsearchCrudJsonWriter, entityInfo, prop, true);
					}
					else
					{
						MapIEnumerableEntities(elasticsearchCrudJsonWriter, ienumerable, entityInfo, false);
					}
				}
			}
		}

		private void MapIEnumerableEntitiesForChildIndexes(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, IEnumerable ienumerable, EntityContextInfo parentEntityInfo,PropertyInfo prop, bool createPropertyMappings)
		{
			if (createPropertyMappings)
			{
				object item;
				if (prop.PropertyType.GenericTypeArguments.Length == 0)
				{
					item = Activator.CreateInstance(prop.PropertyType.GetElementType());
				}
				else
				{
					item = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
				}

				CreateChildEntityForDocumentIndex(parentEntityInfo, elasticsearchCrudJsonWriter, item, true);
			}
			else
			{
				if (ienumerable != null)
				{
					foreach (var item in ienumerable)
					{
						CreateChildEntityForDocumentIndex(parentEntityInfo, elasticsearchCrudJsonWriter, item, false);
					}
				}
			} 			
		}

		private void MapIEnumerableEntitiesForMapping(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter,
			 EntityContextInfo parentEntityInfo, PropertyInfo prop, bool createPropertyMappings)
		{
			object item;
			if (prop.PropertyType.GenericTypeArguments.Length == 0)
			{
				item = Activator.CreateInstance(prop.PropertyType.GetElementType());
			}
			else
			{
				item = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
			}

			var typeofArrayItem = item.GetType();
			if (typeofArrayItem.IsClass && typeofArrayItem.FullName != "System.String" &&
				typeofArrayItem.FullName != "System.Decimal")
			{
				// collection of Objects
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				if (Attribute.IsDefined(prop, typeof(ElasticsearchNestedAttribute)))
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("type");
					elasticsearchCrudJsonWriter.JsonWriter.WriteValue("nested");

					object[] attrs = prop.GetCustomAttributes(typeof(ElasticsearchNestedAttribute), true);

					if ((attrs[0] as ElasticsearchNestedAttribute) != null)
					{
						(attrs[0] as ElasticsearchNestedAttribute).WriteJson(elasticsearchCrudJsonWriter);
					}
				}

				// "properties": {
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("properties");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

	
				// Do class mapping for nested type
				var routingDefinition = new RoutingDefinition
				{
					ParentId = parentEntityInfo.Id,
					RoutingId = parentEntityInfo.RoutingDefinition.RoutingId
				};
				var child = new EntityContextInfo
				{
					Document = item,
					RoutingDefinition = routingDefinition,
					EntityType = item.GetType(),
					DeleteDocument = parentEntityInfo.DeleteDocument
				};
				MapEntityValues(child, elasticsearchCrudJsonWriter, false, createPropertyMappings);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			else
			{
				// {"type": "ienumerable"}
				// collection of simple types
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("type");
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(GetElasticsearchType(item.GetType()));
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		private void MapIEnumerableEntities(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, IEnumerable ienumerable, EntityContextInfo parentEntityInfo, bool createPropertyMappings)
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
						var routingDefinition = new RoutingDefinition { ParentId = parentEntityInfo.Id, RoutingId = parentEntityInfo.RoutingDefinition.RoutingId };
						var child = new EntityContextInfo { Document = item, RoutingDefinition = routingDefinition, EntityType = item.GetType(), DeleteDocument = parentEntityInfo.DeleteDocument };
						MapEntityValues(child, childElasticsearchCrudJsonWriter, false, createPropertyMappings);
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

		/// <summary>
		/// Override this if you require a special type definitoin for your document type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>The type used in Elasticsearch for this type</returns>
		public virtual string GetDocumentType(Type type)
		{
			// Adding support for EF types
			if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
			{
				type = type.BaseType;
			}
			return type.Name.ToLower();
		}

		public virtual Type GetEntityDocumentType(Type type)
		{
			// Adding support for EF types
			if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
			{
				type = type.BaseType;
			}
			return type;
		}

		/// <summary>
		/// Overide this if you need to define the index for your document. 
		/// Required if your using a child document type.
		/// Default: pluralize the default type
		/// </summary>
		/// <param name="type">Type of class used</param>
		/// <returns>The index used in Elasticsearch for this type</returns>
		public virtual string GetIndexForType(Type type)
		{
			// Adding support for EF types
			if (type.BaseType != null && type.Namespace == "System.Data.Entity.DynamicProxies")
			{
				type = type.BaseType;
			}
			return type.Name.ToLower() + "s";
		}

		/// <summary>
		/// bool System.Boolean
		/// byte System.Byte
		/// sbyte System.SByte 
		/// char System.Char
		/// decimal System.Decimal => string
		/// double System.Double
		/// float System.Single
		/// int System.Int32
		/// uint System.UInt32
		/// long System.Int64
		/// ulong System.UInt64
		/// short System.Int16
		/// ushort System.UInt16
		/// string System.String 
		/// </summary>
		/// <param name="propertyType"></param>
		/// <returns>
		/// string,  boolean, and null.
		/// float, double, byte, short, integer, and long
		/// date
		/// binary
		/// </returns>
		public string GetElasticsearchType(Type propertyType)
		{
			switch (propertyType.FullName)
			{
				case "System.Boolean":
					return "boolean";
				case "System.Byte":
					return "byte";
				case "System.SByte":
					return "byte";
				case "System.Double":
					return "double";
				case "System.Single":
					return "float";
				case "System.Int32":
					return "integer";
				case "System.UInt32":
					return "integer";
				case "System.Int64":
					return "long";
				case "System.UInt64":
					return "long";
				case "System.Int16":
					return "short";
				case "System.UInt16":
					return "short";
				default:
					return "string";
			}
		}
	}
}
