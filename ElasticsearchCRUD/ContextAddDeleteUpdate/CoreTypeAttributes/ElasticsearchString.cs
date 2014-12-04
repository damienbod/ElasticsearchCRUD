using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property , AllowMultiple = false, Inherited = true)]
	public class ElasticsearchString : ElasticsearchCoreTypes
	{
		private string _indexName;
		private bool _store;
		private StringIndex _index;
		private bool _docValues;
		private TermVector _termVector;
		private double _boost;
		private object _nullValue;
		private bool _normsEnabled;
		private NormsLoading _normsLoading;
		private IndexOptions _indexOptions;
		private string _analyzer;
	    private string _indexAnalyzer;
		private string _searchAnalyzer;
		private bool _includeInAll;
		private string _ignoreAbove;
		private long _positionOffsetGap;

		private bool _indexNameSet;
		private bool _storeSet;
		private bool _indexSet;
		private bool _docValuesSet;
		private bool _termVectorSet;
		private bool _boostSet;
		private bool _nullValueSet;
		private bool _normsEnabledSet;
		private bool _normsLoadingSet;
		private bool _indexOptionsSet;
		private bool _analyzerSet;
		private bool _indexAnalyzerSet;
		private bool _searchAnalyzerSet;
		private bool _includeInAllSet;
		private bool _ignoreAboveSet;
		private bool _positionOffsetGapSet;

		public override string JsonString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", "string", elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("index_name", _indexName, elasticsearchCrudJsonWriter, _indexNameSet);
			JsonHelper.WriteValue("store", _store, elasticsearchCrudJsonWriter, _storeSet);
			JsonHelper.WriteValue("index", _index.ToString(), elasticsearchCrudJsonWriter, _indexSet);
			JsonHelper.WriteValue("doc_values", _docValues, elasticsearchCrudJsonWriter, _docValuesSet);
			JsonHelper.WriteValue("term_vector", _termVector.ToString(), elasticsearchCrudJsonWriter, _termVectorSet);
			JsonHelper.WriteValue("boost", _boost, elasticsearchCrudJsonWriter, _boostSet);
			JsonHelper.WriteValue("null_value", _nullValue, elasticsearchCrudJsonWriter, _nullValueSet);

			//"norms" : {
			//		"enabled" : false
			//	}
			if (_normsEnabledSet || _normsLoadingSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("norms");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				JsonHelper.WriteValue("enabled", _normsEnabled, elasticsearchCrudJsonWriter, _normsEnabledSet);
				JsonHelper.WriteValue("loading", _normsLoading.ToString(), elasticsearchCrudJsonWriter, _normsLoadingSet);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			JsonHelper.WriteValue("index_options", _indexOptions.ToString(), elasticsearchCrudJsonWriter, _indexOptionsSet);
			JsonHelper.WriteValue("analyzer", _analyzer, elasticsearchCrudJsonWriter, _analyzerSet);
			JsonHelper.WriteValue("index_analyzer", _indexAnalyzer, elasticsearchCrudJsonWriter, _indexAnalyzerSet);
			JsonHelper.WriteValue("search_analyzer", _searchAnalyzer, elasticsearchCrudJsonWriter, _searchAnalyzerSet);
			JsonHelper.WriteValue("include_in_all", _includeInAll, elasticsearchCrudJsonWriter, _includeInAllSet);
			JsonHelper.WriteValue("ignore_above", _ignoreAbove, elasticsearchCrudJsonWriter, _ignoreAboveSet);
			JsonHelper.WriteValue("position_offset_gap", _positionOffsetGap, elasticsearchCrudJsonWriter, _positionOffsetGapSet);

			WriteBaseValues(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			return elasticsearchCrudJsonWriter.Stringbuilder.ToString();
		}
	
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
		/// index
		/// Set to analyzed for the field to be indexed and searchable after being broken down into token using an analyzer. not_analyzed means that its still searchable, but does not go through any analysis process or broken down into tokens. no means that it won’t be searchable at all (as an individual field; it may still be included in _all). Setting to no disables include_in_all. Defaults to analyzed.	
		/// </summary>
		public virtual StringIndex Index
		{
			get { return _index; }
			set
			{
				_index = value;
				_indexSet = true;
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
		/// term_vector
		/// Possible values are no, yes, with_offsets, with_positions, with_positions_offsets. Defaults to no.
		/// </summary>
		public virtual TermVector TermVector
		{
			get
			{
				return _termVector;
			}
			set
			{
				_termVector = value;
				_termVectorSet = true;
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

		/// <summary>
		/// norms: {enabled: value}
		/// Boolean value if norms should be enabled or not. Defaults to true for analyzed fields, and to false for not_analyzed fields. See the section about norms.
		/// </summary>
		public virtual bool NormsEnabled
		{
			get { return _normsEnabled; }
			set
			{
				_normsEnabled = value;
				_normsEnabledSet = true;
			}
		}

		/// <summary>
		/// norms: {loading: value}
		/// Describes how norms should be loaded, possible values are eager and lazy (default). It is possible to change the default value to eager for all fields by configuring the index setting index.norms.loading to eager.		
		/// </summary>
		public virtual NormsLoading NormsLoading
		{
			get { return _normsLoading; }
			set
			{
				_normsLoading = value;
				_normsLoadingSet = true;
			}
		}

		/// <summary>
		/// index_options
		/// Allows to set the indexing options, possible values are docs (only doc numbers are indexed), freqs (doc numbers and term frequencies), and positions (doc numbers, term frequencies and positions). Defaults to positions for analyzed fields, and to docs for not_analyzed fields. It is also possible to set it to offsets (doc numbers, term frequencies, positions and offsets).	
		/// </summary>
		public virtual IndexOptions IndexOptions
		{
			get { return _indexOptions; }
			set
			{
				_indexOptions = value;
				_indexOptionsSet = true;
			}
		}

		/// <summary>
		/// analyzer
		/// The analyzer used to analyze the text contents when analyzed during indexing and when searching using a query string. Defaults to the globally configured analyzer.
		/// </summary>
		public virtual string Analyzer
		{
			get { return _analyzer; }
			set
			{
				_analyzer = value;
				_analyzerSet = true;
			}
		}

		/// <summary>
		/// index_analyzer
		/// The analyzer used to analyze the text contents when analyzed during indexing.
		/// </summary>
		public virtual string IndexAnalyzer
		{
			get { return _indexAnalyzer; }
			set
			{
				_indexAnalyzer = value;
				_indexAnalyzerSet = true;
			}
		}

		/// <summary>
		/// search_analyzer
		/// The analyzer used to analyze the field when part of a query string. Can be updated on an existing field.
		/// </summary>
		public virtual string SearchAnalyzer
		{
			get { return _searchAnalyzer; }
			set
			{
				_searchAnalyzer = value;
				_searchAnalyzerSet = true;
			}
		}

		/// <summary>
		/// include_in_all
		/// Should the field be included in the _all field (if enabled). If index is set to no this defaults to false, otherwise, defaults to true or to the parent object type setting.
		/// </summary>
		public virtual bool IncludeInAll
		{
			get { return _includeInAll; }
			set
			{
				_includeInAll = value;
				_includeInAllSet = true;
			}
		}

		/// <summary>
		/// ignore_above
		/// The analyzer will ignore strings larger than this size. Useful for generic not_analyzed fields that should ignore long text.
		/// </summary>
		public virtual string IgnoreAbove
		{
			get { return _ignoreAbove; }
			set
			{
				_ignoreAbove = value;
				_ignoreAboveSet = true;
			}
		}

		/// <summary>
		/// position_offset_gap
		/// Position increment gap between field instances with the same field name. Defaults to 0.
		/// </summary>
		public virtual long PositionOffsetGap
		{
			get { return _positionOffsetGap; }
			set
			{
				_positionOffsetGap = value;
				_positionOffsetGapSet = true;
			}
		}

	}

	public enum StringIndex
	{
		not_analyzed,
		analyzed,
		no
	}

	//  no, yes, with_offsets, with_positions, with_positions_offsets. Defaults to no.
	public enum TermVector
	{
		no,
		yes, 
		with_offsets, 
		with_positions, 
		with_positions_offsets
	}

	public enum NormsLoading
	{
		eager,
		lazy
	}

	public enum IndexOptions
	{
		docs,
		freqs,
		positions,
		offsets
	}

}
