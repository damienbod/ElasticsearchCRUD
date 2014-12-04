using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
	public class ElasticsearchBinary : ElasticsearchCoreTypes
	{
		private string _indexName;
		private bool _store;
		private bool _docValues;

		private bool _compress;
		private int _compressThreshold;		

		private bool _indexNameSet;
		private bool _storeSet;
		private bool _docValuesSet;
		private bool _compressSet;
		private bool _compressThresholdSet;

		/// <summary>
		/// index_name
		/// The name of the field that will be stored in the index. Defaults to the property/field name.
		/// </summary>
		public virtual string IndexName
		{
			get { return _indexName; }
			set
			{
				_indexName = value;
				_indexNameSet = true;
			}
		}

		/// <summary>
		/// compress
		/// Set to true to compress the stored binary value.
		/// </summary>
		public virtual bool Compress
		{
			get { return _compress; }
			set
			{
				_compress = value;
				_compressSet = true;
			}
		}

		/// <summary>
		/// store
		/// Set to true to actually store the field in the index, false to not store it. Defaults to false (note, the JSON document itself is stored, and it can be retrieved from it).
		/// </summary>
		public virtual bool Store
		{
			get { return _store; }
			set
			{
				_store = value;
				_storeSet = true;
			}
		}

		/// <summary>
		/// doc_values
		/// Set to true to store field values in a column-stride fashion. Automatically set to true when the fielddata format is doc_values.
		/// </summary>
		public virtual bool DocValues
		{
			get { return _docValues; }
			set
			{
				_docValues = value;
				_docValuesSet = true;
			}
		}

		/// <summary>
		/// //compress_threshold
		/// Compression will only be applied to stored binary fields that are greater than this size. Defaults to -1 
		/// </summary>
		public virtual int CompressThreshold
		{
			get { return _compressThreshold; }
			set
			{
				_compressThreshold = value;
				_compressThresholdSet = true;
			}
		}

		public override string JsonString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", "binary", elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("compress_threshold", _compressThreshold, elasticsearchCrudJsonWriter, _compressThresholdSet);
			JsonHelper.WriteValue("index_name", _indexName, elasticsearchCrudJsonWriter, _indexNameSet);
			JsonHelper.WriteValue("store", _store, elasticsearchCrudJsonWriter, _storeSet);
			JsonHelper.WriteValue("doc_values", _docValues, elasticsearchCrudJsonWriter, _docValuesSet);
			JsonHelper.WriteValue("compress", _compress, elasticsearchCrudJsonWriter, _compressSet);

			WriteBaseValues(elasticsearchCrudJsonWriter);
			
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			return elasticsearchCrudJsonWriter.Stringbuilder.ToString();
		}
	}
}

