using Newtonsoft.Json;

namespace Damienbod.ElasticSearchProvider
{
	public abstract class ElasticSearchSerializerMapping<T>
	{
		protected JsonWriter Writer;

		public abstract void MapEntityValues(T entity);
		public abstract T ParseEntity(Newtonsoft.Json.Linq.JToken source);
	
		public void AddWriter(JsonWriter writer)
		{
			Writer = writer;
		}

		protected void MapValue(string key, object valueObj)
		{
			Writer.WritePropertyName(key);
			Writer.WriteValue(valueObj);
		}


	}
}
