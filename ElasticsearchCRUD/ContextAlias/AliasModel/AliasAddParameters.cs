using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAlias.AliasModel
{
	public class AliasAddParameters : AliasBaseParameters
	{
		private string _routing;
		private string _filter;
		private bool _routingSet;
		private bool _filterSet;

		public AliasAddParameters(string alias, string index) : base(alias, index)
		{
		}

		public string Routing
		{
			get { return _routing; }
			set
			{
				_routing = value;
				_routingSet = true;
			}
		}

		public string Filter
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
			JsonHelper.WriteValue("filter", _filter, elasticsearchCrudJsonWriter, _filterSet);
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteInternalJson(elasticsearchCrudJsonWriter, AliasAction.add, WriteValues);
		}
	}
}