using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class ElasticsearchCoreTypes : Attribute
	{
		public virtual string JsonString()
		{
			return "";
		}

		protected void WriteValue(string key, object valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
			}
		}
	}
}