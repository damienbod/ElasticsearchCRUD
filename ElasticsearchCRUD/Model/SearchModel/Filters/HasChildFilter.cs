using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class HasChildFilter : IFilter
	{
		private readonly string _type;
		private readonly IFilter _filter;
		private uint _minChildren;
		private bool _minChildrenSet;
		private uint _maxChildren;
		private bool _maxChildrenSet;

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

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
