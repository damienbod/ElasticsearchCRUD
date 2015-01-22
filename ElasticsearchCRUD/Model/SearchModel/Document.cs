using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	public class Document
	{
		private string _index;
		private bool _indexSet;
		private string _type;
		private bool _typeSet;
		private object _id;
		private bool _idSet;
		private string _routing;
		private bool _routingSet;

		public string Index
		{
			get { return _index; }
			set
			{
				_index = value;
				_indexSet = true;
			}
		}

		public string Type
		{
			get { return _type; }
			set
			{
				_type = value;
				_typeSet = true;
			}
		}

		public object Id
		{
			get { return _id; }
			set
			{
				_id = value;
				_idSet = true;
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

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("_index", _index, elasticsearchCrudJsonWriter, _indexSet);
			JsonHelper.WriteValue("_type", _type, elasticsearchCrudJsonWriter, _typeSet);
			JsonHelper.WriteValue("_id", _id, elasticsearchCrudJsonWriter, _idSet);
			JsonHelper.WriteValue("_routing", _routing, elasticsearchCrudJsonWriter, _routingSet);
		}
	}
}