using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	/// <summary>
	/// This model is only used when creating an index. If it is required that an index is added later, use the alias api.
	/// </summary>
	public class IndexAlias
	{
		private readonly string _alias;
		private string _routing;
		private string _filter;
		private bool _routingSet;
		private bool _filterSet;

		public IndexAlias(string alias)
		{
			MappingUtils.GuardAgainstBadIndexName(alias);

			_alias = alias;
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
		/// TODO replace this raw json string with a filter object once the filter class has been created.
		/// </summary>
		public string Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				_filterSet = true;
			}
		}

		//"aliases" : {
		//  "april_2014" : {},
		//  "year_2014" : {}
		//}, 
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_alias);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("routing", _routing, elasticsearchCrudJsonWriter, _routingSet);
			if (_filterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue(_filter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();			
		}
	}
}