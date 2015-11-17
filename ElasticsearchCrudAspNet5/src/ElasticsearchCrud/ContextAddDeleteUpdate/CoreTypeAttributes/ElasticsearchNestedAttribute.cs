using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	/// <summary>
	///  This attribute is used if you need to change the structure of the nested objects in Elastissearch to nested nested objects.
	/// 
	/// Nested fields may contain other nested fields. 
	/// The include_in_parent object refers to the direct parent of the field, while the include_in_root parameter 
	/// refers only to the topmost “root” object or document.
	/// 
	/// Nested docs will automatically use the root doc _all field only.
	/// 
	/// Internally, nested objects are indexed as additional documents, but, since they can be guaranteed to be indexed within the same "block", 
	/// it allows for extremely fast joining with parent docs.
	/// 
	/// Those internal nested documents are automatically masked away when doing operations against the index (like searching with a match_all query), 
	/// and they bubble out when using the nested query.
	/// 
	/// Because nested docs are always masked to the parent doc, the nested docs can never be accessed outside the scope of the nested query. 
	/// For example stored fields can be enabled on fields inside nested objects, but there is no way of retrieving them, 
	/// since stored fields are fetched outside of the nested query scope.
	/// 
	/// The _source field is always associated with the parent document and because of that field values via the source can be fetched for nested object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ElasticsearchNestedAttribute : Attribute
	{
		private bool _includeInParent;
		private bool _includeInParentSet;
		private bool _includeInRoot;
		private bool _includeInRootSet;

		/// <summary>
		/// include_in_parent
		/// You may want to index inner objects both as nested fields and as flattened object fields, eg for highlighting. This can be achieved by setting include_in_parent to true:
		/// </summary>
		public bool IncludeInParent
		{
			get { return _includeInParent; }
			set
			{
				_includeInParent = value;
				_includeInParentSet = true;
			}
		}

		/// <summary>
		/// include_in_root
		/// 
		/// </summary>
		public bool IncludeInRoot
		{
			get { return _includeInRoot; }
			set
			{
				_includeInRoot = value;
				_includeInRootSet = true;
			}
		}
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("include_in_parent", _includeInParent, elasticsearchCrudJsonWriter, _includeInParentSet);
			JsonHelper.WriteValue("include_in_root", _includeInRoot, elasticsearchCrudJsonWriter, _includeInRootSet);
		}
	}
}
