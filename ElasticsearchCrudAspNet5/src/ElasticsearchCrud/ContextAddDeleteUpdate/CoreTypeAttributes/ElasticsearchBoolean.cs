using System;
using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
	public class ElasticsearchBoolean : ElasticsearchCoreTypes
	{
		private string _indexName;
		private NumberIndex _index;
		private bool _store;
		private bool _docValues;
		private double _boost;
		private object _nullValue;		

		private bool _indexNameSet;
		private bool _storeSet;
		private bool _docValuesSet;
		private bool _boostSet;
		private bool _nullValueSet;
		private bool _indexSet;
		
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
		/// index
		/// Set to no if the value should not be indexed. Setting to no disables include_in_all. If set to no the field should be either stored in _source, have include_in_all enabled, or store be set to true for this to be useful.
		/// </summary>
		public virtual NumberIndex Index
		{
			get { return _index; }
			set
			{
				_index = value;
				_indexSet = true;
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
		/// boost
		/// The boost value. Defaults to 1.0.
		/// </summary>
		public virtual double Boost
		{
			get { return _boost; }
			set
			{
				_boost = value;
				_boostSet = true;
			}
		}

		/// <summary>
		/// null_value
		/// When there is a (JSON) null value for the field, use the null_value as the field value. Defaults to not adding the field at all.
		/// </summary>
		public virtual object NullValue
		{
			get { return _nullValue; }
			set
			{
				_nullValue = value;
				_nullValueSet = true;
			}
		}

		public override string JsonString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", "boolean", elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("index", _index.ToString(), elasticsearchCrudJsonWriter, _indexSet);
			JsonHelper.WriteValue("index_name", _indexName, elasticsearchCrudJsonWriter, _indexNameSet);
			JsonHelper.WriteValue("store", _store, elasticsearchCrudJsonWriter, _storeSet);
			JsonHelper.WriteValue("doc_values", _docValues, elasticsearchCrudJsonWriter, _docValuesSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			JsonHelper.WriteValue("null_value", _nullValue, elasticsearchCrudJsonWriter, _nullValueSet);

			WriteBaseValues(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			return elasticsearchCrudJsonWriter.Stringbuilder.ToString();
		}
	}
}

