using System;
using Newtonsoft.Json;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Default mapping for your Entity. You can implement this clas to implement your specific mapping if required
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ElasticSearchSerializerMapping<T>
	{
		protected JsonWriter Writer;

		public virtual void MapEntityValues(T entity)
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

		public virtual T ParseEntity(Newtonsoft.Json.Linq.JToken source)
		{
			return (T)JsonConvert.DeserializeObject(source.ToString(), typeof(T));
		}

		public virtual string GetDocumentType(Type type)
		{
			return type.Name;
		}


	}
}
