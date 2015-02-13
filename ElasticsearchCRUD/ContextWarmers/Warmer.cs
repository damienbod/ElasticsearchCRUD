using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel;

namespace ElasticsearchCRUD.ContextWarmers
{
	public class Warmer
	{
		private IQueryHolder _query;
		private bool _querySet;
		private List<IAggs> _aggs;
		private bool _aggsSet;

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

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
