using Newtonsoft.Json;

namespace Damienbod.ElasticSearchProvider
{
	public class ElasticSearchSerializerMapping<T>
	{
		protected JsonWriter Writer;

		public virtual void WriteJsonEntry(T entity)
		{
		}

		public void AddWriter(JsonWriter writer)
		{
			Writer = writer;
		}

		protected void WriteValue(string key, object valueObj)
		{
			Writer.WritePropertyName(key);
			Writer.WriteValue(valueObj);
		}
	}
}
