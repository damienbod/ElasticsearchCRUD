using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	public class Filter : IFilterHolder
	{
		private readonly IFilter _filter;
		private string _name;
		private bool _nameSet;

		public Filter(IFilter filter)
		{
			_filter = filter;
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				_nameSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_filter.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("_name", _name,elasticsearchCrudJsonWriter,_nameSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}