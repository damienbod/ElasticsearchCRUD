using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// The has_child filter accepts a query and the child type to run against, and results in parent documents that have child docs matching the query.
	/// 
	/// The type is the child type to query against. The parent type to return is automatically detected based on the mappings.
	/// 
	/// The way that the filter is implemented is by first running the child query, doing the matching up to the parent doc for each document matched.
	/// 
	/// The has_child filter allows you to specify that a minimum and/or maximum number of children are required to match for the parent doc to be considered a match:
	/// </summary>
	public class HasChildFilter : IFilter
	{
		private readonly string _type;
		private readonly IFilter _filter;
		private uint _minChildren;
		private bool _minChildrenSet;
		private uint _maxChildren;
		private bool _maxChildrenSet;
		private InnerHits _innerHits;
		private bool _innerHitsSet;

		public HasChildFilter(string type, IFilter filter)
		{
			_type = type;
			_filter = filter;
		}

		/// <summary>
		/// min_children
		/// 
		/// </summary>
		public uint MinChildren
		{
			get { return _minChildren; }
			set
			{
				_minChildren = value;
				_minChildrenSet = true;
			}
		}

		/// <summary>
		/// max_children
		/// </summary>
		public uint MaxChildren
		{
			get { return _maxChildren; }
			set
			{
				_maxChildren = value;
				_maxChildrenSet = true;
			}
		}

		public InnerHits InnerHits
		{
			get { return _innerHits; }
			set
			{
				_innerHits = value;
				_innerHitsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("has_child");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", _type, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_filter.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("min_children", _minChildren, elasticsearchCrudJsonWriter, _minChildrenSet);
			JsonHelper.WriteValue("max_children", _maxChildren, elasticsearchCrudJsonWriter, _maxChildrenSet);

			if (_innerHitsSet)
			{
				_innerHits.WriteJson(elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
