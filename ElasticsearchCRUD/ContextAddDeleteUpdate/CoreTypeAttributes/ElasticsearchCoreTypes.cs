using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class ElasticsearchCoreTypes : Attribute
	{
		protected string _similarity;
		protected bool _similaritySet;
		protected string _copyTo;
		protected bool _copyToSet;

		public virtual string JsonString()
		{
			return "";
		}

		/// <summary>
		/// "similarity":"BM25"
		/// </summary>
		public virtual string Similarity
		{
			get { return _similarity; }
			set
			{
				_similarity = value;
				_similaritySet = true;
			}
		}

		public virtual string CopyTo
		{
			get { return _copyTo; }
			set
			{
				_copyTo = value;
				_copyToSet = true;
			}
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