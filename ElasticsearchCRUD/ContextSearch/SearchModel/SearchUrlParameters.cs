namespace ElasticsearchCRUD.ContextSearch.SearchModel
{
	/// <summary>
	/// This class is used to set routing or pretty search for the url parameters. All other options can be sent in the body
	/// </summary>
	public class SearchUrlParameters
	{
		private bool _pretty;
		private bool _prettySet;
		private string _routing;
		private bool _routingSet;

		public bool Pretty
		{
			get { return _pretty; }
			set
			{
				_pretty = value;
				_prettySet = true;
			}
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

		public string GetUrlParameters()
		{
			if (_routingSet && _prettySet)
			{
				return "?routing=" + _routing + "&pretty=true";
			}
			if (_routingSet )
			{
				return "?routing=" + _routing;
			}
			if (_prettySet)
			{
				return "?pretty=true";
			}

			return "";
		}
	}
}
