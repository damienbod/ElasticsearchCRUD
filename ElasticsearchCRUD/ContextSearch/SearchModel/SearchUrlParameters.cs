using ElasticsearchCRUD.Utils;

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
			var parameters = new ParameterCollection();
			if (_routingSet )
			{
				parameters.Add("routing", _routing);
			}
			if (_prettySet)
			{
				parameters.Add("pretty", "true");
			}

			return parameters.ToString();
		}
	}
}
