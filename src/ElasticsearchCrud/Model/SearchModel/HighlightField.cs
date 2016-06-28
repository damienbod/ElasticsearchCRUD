using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	public class HighlightField
	{
		private readonly string _field;
		private uint _fragmentSize;
		private bool _fragmentSizeSet;
		private uint _numberOfFragments;
		private bool _numberOfFragmentsSet;
		private IQuery _highlightQuery;
		private bool _highlightQuerySet;
		private List<string> _preTags;
		private bool _preTagsSet;
		private List<string> _postTags;
		private bool _postTagsSet;
		private List<string> _matchedFields;
		private bool _matchedFieldsSet;
		private HighlightFieldType _fieldType;
		private bool _fieldTypeSet;
		private bool _forceSource;
		private bool _forceSourceSet;
		private uint _noMatchSize;
		private bool _noMatchSizeSet;
		private HighlightEncoder _encoder;
		private bool _encoderSet;
		private bool _orderByScore;
		private bool _orderByScoreSet;
		private uint _fragmentOffset;
		private bool _fragmentOffsetSet;

		public HighlightField(string field)
		{
			_field = field;
		}

		/// <summary>
		/// fragment_size 
		/// Each field highlighted can control the size of the highlighted fragment in characters (defaults to 100), and the maximum number of fragments to return (defaults to 5).
		/// </summary>
		public uint FragmentSize
		{
			get { return _fragmentSize; }
			set
			{
				_fragmentSize = value;
				_fragmentSizeSet = true;
			}
		}

		/// <summary>
		/// number_of_fragments
		/// Each field highlighted can control the size of the highlighted fragment in characters (defaults to 100), and the maximum number of fragments to return (defaults to 5).
		/// If the number_of_fragments value is set to 0 then no fragments are produced, instead the whole content of the field is returned, 
		/// and of course it is highlighted. This can be very handy if short texts (like document title or address) need to be highlighted but no fragmentation is required. 
		/// Note that fragment_size is ignored in this case.
		/// </summary>
		public uint NumberOfFragments
		{
			get { return _numberOfFragments; }
			set
			{
				_numberOfFragments = value;
				_numberOfFragmentsSet = true;
			}
		}

		/// <summary>
		/// An encoder parameter can be used to define how highlighted text will be encoded. 
		/// It can be either default (no encoding) or html (will escape html, if you use html highlighting tags).
		/// </summary>
		public HighlightEncoder Encoder
		{
			get { return _encoder; }
			set
			{
				_encoder = value;
				_encoderSet = true;
			}
		}

		 

		/// <summary>
		/// highlight_query
		/// It is also possible to highlight against a query other than the search query by setting highlight_query. 
		/// This is especially useful if you use a rescore query because those are not taken into account by highlighting by default. 
		/// Elasticsearch does not validate that highlight_query contains the search query in any way so it is possible to define it so legitimate query results aren’t highlighted at all. 
		/// Generally it is better to include the search query in the highlight_query. 
		/// </summary>
		public IQuery HighlightQuery
		{
			get { return _highlightQuery; }
			set
			{
				_highlightQuery = value;
				_highlightQuerySet = true;
			}
		}

		/// <summary>
		/// pre_tags
		/// </summary>
		public List<string> PreTags
		{
			get { return _preTags; }
			set
			{
				_preTags = value;
				_preTagsSet = true;
			}
		}

		/// <summary>
		/// post_tags
		/// </summary>
		public List<string> PostTags
		{
			get { return _postTags; }
			set
			{
				_postTags = value;
				_postTagsSet = true;
			}
		}

		/// <summary>
		/// matched_fields
		/// The Fast Vector Highlighter can combine matches on multiple fields to highlight a single field using matched_fields. 
		/// This is most intuitive for multifields that analyze the same string in different ways. 
		/// All matched_fields must have term_vector set to with_positions_offsets but only the field to which the matches are combined is loaded 
		/// so only that field would benefit from having store set to yes.
		/// </summary>
		public List<string> MatchedFields
		{
			get { return _matchedFields; }
			set
			{
				_matchedFields = value;
				_matchedFieldsSet = true;
			}
		}

		/// <summary>
		/// The type field allows to force a specific highlighter type. 
		/// This is useful for instance when needing to use the plain highlighter on a field that has term_vectors enabled. 
		/// The allowed values are: plain, postings and fvh. 
		/// </summary>
		public HighlightFieldType FieldType
		{
			get { return _fieldType; }
			set
			{
				_fieldType = value;
				_fieldTypeSet = true;
			}
		}

		/// <summary>
		/// order" : "score",
		/// </summary>
		public bool OrderByScore
		{
			get { return _orderByScore; }
			set
			{
				_orderByScore = value;
				_orderByScoreSet = true;
			}
		}
		
		/// <summary>
		/// force_source 
		/// Forces the highlighting to highlight fields based on the source even if fields are stored separately. Defaults to false.
		/// </summary>
		public bool ForceSource
		{
			get { return _forceSource; }
			set
			{
				_forceSource = value;
				_forceSourceSet = true;
			}
		}
		
		/// <summary>
		/// no_match_size
		/// </summary>
		public uint NoMatchSize
		{
			get { return _noMatchSize; }
			set
			{
				_noMatchSize = value;
				_noMatchSizeSet = true;
			}
		}

		/// <summary>
		/// When using fast-vector-highlighter one can use fragment_offset parameter to control the margin to start highlighting from.
		/// 
		/// In the case where there is no matching fragment to highlight, the default is to not return anything.
		///  Instead, we can return a snippet of text from the beginning of the field by setting no_match_size (default 0) to the length of the text that you want returned. 
		/// The actual length may be shorter than specified as it tries to break on a word boundary. 
		/// When using the postings highlighter it is not possible to control the actual size of the snippet, 
		/// therefore the first sentence gets returned whenever no_match_size is greater than 0.
		/// </summary>
		public uint FragmentOffset
		{
			get { return _fragmentOffset; }
			set
			{
				_fragmentOffset = value;
				_fragmentOffsetSet = true;
			}
		}
		

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("fragment_size", _fragmentSize, elasticsearchCrudJsonWriter, _fragmentSizeSet);
			JsonHelper.WriteValue("number_of_fragments", _numberOfFragments, elasticsearchCrudJsonWriter, _numberOfFragmentsSet);
			if (_highlightQuerySet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("highlight_query");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();	
				_highlightQuery.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			JsonHelper.WriteListValue("pre_tags", _preTags, elasticsearchCrudJsonWriter, _preTagsSet);
			JsonHelper.WriteListValue("post_tags", _postTags, elasticsearchCrudJsonWriter, _postTagsSet);
			JsonHelper.WriteListValue("matched_fields", _matchedFields, elasticsearchCrudJsonWriter, _matchedFieldsSet);
			JsonHelper.WriteValue("type", _fieldType.ToString(), elasticsearchCrudJsonWriter, _fieldTypeSet);
			JsonHelper.WriteValue("force_source", _forceSource, elasticsearchCrudJsonWriter, _forceSourceSet);
			JsonHelper.WriteValue("no_match_size", _noMatchSize, elasticsearchCrudJsonWriter, _noMatchSizeSet);

			JsonHelper.WriteValue("encoder", _encoder.ToString(), elasticsearchCrudJsonWriter, _encoderSet);
			JsonHelper.WriteValue("order", "score", elasticsearchCrudJsonWriter, _orderByScoreSet);
			JsonHelper.WriteValue("fragment_offset", _fragmentOffset, elasticsearchCrudJsonWriter, _fragmentOffsetSet);
			
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum HighlightEncoder	
	{
		@default, html
	}

	public enum HighlightFieldType
	{
		plain, fvh, postings 
	}
}