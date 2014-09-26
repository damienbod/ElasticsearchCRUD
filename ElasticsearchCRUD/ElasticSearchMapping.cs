using System;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Default mapping for your Entity. You can implement this clas to implement your specific mapping if required
	/// </summary>
	public class ElasticSearchMapping
	{
		protected JsonWriter Writer;

		public virtual void MapEntityValues(object entity)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach (var prop in propertyInfo)
			{
				MapValue(prop.Name, prop.GetValue(entity));
			}
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

		public virtual object ParseEntity(Newtonsoft.Json.Linq.JToken source, Type type)
		{
			return JsonConvert.DeserializeObject(source.ToString(), type);
		}

		public virtual string GetDocumentType(Type type)
		{
			return type.Name.ToLower();
		}

		public virtual string GetIndexForType(Type type)
		{
			return type.Name.ToLower();
		}
	}

}
