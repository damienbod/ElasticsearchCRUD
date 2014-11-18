using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class ElasticsearchCoreTypes : Attribute
	{
		private string _similarity;
		private bool _similaritySet;
		private string _copyTo;
		private List<string> _copyToList;
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

		public virtual List<string> CopyToList
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
			else
			{
				// TODO write string array
				//WriteValue("copy_to", _copyToList, elasticsearchCrudJsonWriter, _copyToListSet);
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