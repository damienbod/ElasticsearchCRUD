using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class ElasticsearchCoreTypes : Attribute
	{
		private string _similarity;
		private bool _similaritySet;
		private string _copyTo;
		private string[] _copyToList;
		private bool _copyToSet;
		private bool _copyToListSet;

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

		public virtual string[] CopyToList
		{
			get { return _copyToList; }
			set
			{
				_copyToList = value;
				_copyToListSet = true;
			}
		}

		protected void WriteBaseValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteValue("similarity", _similarity, elasticsearchCrudJsonWriter, _similaritySet);
			if (_copyToSet)
			{
				WriteValue("copy_to", _copyTo, elasticsearchCrudJsonWriter, _copyToSet);
			}
			else if (_copyToListSet)
			{
				var json = JsonConvert.SerializeObject(_copyToList);
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("copy_to");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue(json);
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