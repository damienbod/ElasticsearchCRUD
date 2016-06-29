using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class NestedFilter : IFilter
	{
		private readonly IFilter _filter;
		private readonly string _path;
		private bool _join;
		private bool _joinSet;
		private InnerHits _innerHits;
		private bool _innerHitsSet;

		public NestedFilter(IFilter filter, string path)
		{
			_filter = filter;
			_path = path;
		}

		public bool Join
		{
			get { return _join; }
			set
			{
				_join = value;
				_joinSet = true;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("nested");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("path", _path, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_filter.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("join", _join, elasticsearchCrudJsonWriter, _joinSet);

			if (_innerHitsSet)
			{
				_innerHits.WriteJson(elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
