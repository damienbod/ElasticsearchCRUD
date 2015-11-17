using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAlias.AliasModel
{
	/// <summary>
	/// APIs in elasticsearch accept an index name when working against a specific index, and several indices when applicable. 
	/// The index aliases API allow to alias an index with a name, with all APIs automatically converting the alias name to the actual index name. 
	/// An alias can also be mapped to more than one index, and when specifying it, the alias will automatically expand to the aliases indices. 
	/// An alias can also be associated with a filter that will automatically be applied when searching, and routing values.
	/// </summary>
	public class AliasAddParameters : AliasBaseParameters
	{
		private string _routing;
		private IFilter _filter;
		private bool _routingSet;
		private bool _filterSet;

		public AliasAddParameters(string alias, string index) : base(alias, index)
		{
		}

		/// <summary>
		/// It is possible to associate routing values with aliases. This feature can be used together with filtering aliases in order to avoid unnecessary shard operations.
		/// </summary>
		public string Routing
		{
			get { return _routing; }
			set
			{
				_routing = value;
				_routingSet = true;
			}
		}

		/// <summary>
		/// An optional filter that can be associated with an alias.
		/// </summary>
		public IFilter Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				_filterSet = true;
			}
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("routing", _routing, elasticsearchCrudJsonWriter, _routingSet);
			if (_filterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_filter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteInternalJson(elasticsearchCrudJsonWriter, AliasAction.add, WriteValues);
		}
	}
}