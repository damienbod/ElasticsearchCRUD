using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class IndexWarmer
	{
		private readonly string _name;
		private IQueryHolder _query;
		private bool _querySet;
		private List<IAggs> _aggs;
		private bool _aggsSet;
		private List<string> _indexTypes;
		private bool _indexTypesSet;

		public IndexWarmer(string name)
		{
			_name = name;
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

		public List<string> IndexTypes
		{
			get { return _indexTypes; }
			set
			{
				_indexTypes = value;
				_indexTypesSet = true;
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
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteListValue("types", _indexTypes, elasticsearchCrudJsonWriter, _indexTypesSet);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("source");
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
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}