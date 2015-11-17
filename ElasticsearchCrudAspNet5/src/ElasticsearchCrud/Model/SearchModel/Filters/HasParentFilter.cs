using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// The has_parent filter accepts a query and a parent type. The query is executed in the parent document space, which is specified by the parent type. 
	/// This filter returns child documents which associated parents have matched. 
	/// For the rest has_parent filter has the same options and works in the same manner as the has_child filter.
	/// </summary>
	public class HasParentFilter : IFilter
	{
		private readonly string _type;
		private readonly IFilter _filter;
		private InnerHits _innerHits;
		private bool _innerHitsSet;

		public HasParentFilter(string type, IFilter filter)
		{
			_type = type;
			_filter = filter;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("has_parent");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", _type, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_filter.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (_innerHitsSet)
			{
				_innerHits.WriteJson(elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}