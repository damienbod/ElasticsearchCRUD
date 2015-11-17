using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextWarmers
{
	public class Warmer
	{
		public string Name { get; private set; }
		private IQueryHolder _query;
		private bool _querySet;
		private List<IAggs> _aggs;
		private bool _aggsSet;
		private bool _queryCache;
		private bool _queryCacheSet;

		public Warmer(string name)
		{
			Name = name;
		}

		public IQueryHolder Query
		{
			get { return _query; }
			set
			{
				_query = value;
				_querySet = true;
			}
		}

		/// <summary>
		/// query_cache
		/// </summary>
		public bool QueryCache
		{
			get { return _queryCache; }
			set
			{
				_queryCache = value;
				_queryCacheSet = true;
			}
		}
		
		/// <summary>
		/// aggregations request
		/// </summary>
		public List<IAggs> Aggs
		{
			get { return _aggs; }
			set
			{
				_aggs = value;
				_aggsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (_querySet)
			{
				_query.WriteJson(elasticsearchCrudJsonWriter);
			}

			if (_aggsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				foreach (var item in _aggs)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			JsonHelper.WriteValue("query_cache", _queryCache, elasticsearchCrudJsonWriter, _queryCacheSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		public override string ToString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			WriteJson(elasticsearchCrudJsonWriter);
			return elasticsearchCrudJsonWriter.GetJsonString();
		}
	}
}
