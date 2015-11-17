using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class IndicesQuery : IQuery
	{
		private readonly List<string> _indices;
		private readonly IQuery _query;
		private IQuery _noMatchQuery;
		private bool _noMatchQuerySet;
		private bool _noMatchFilterNone;
		private bool _noMatchFilterNoneSet;

		public IndicesQuery(List<string> indices, IQuery query)
		{
			_indices = indices;
			_query = query;
		}

		/// <summary>
		/// no_match_query
		/// </summary>
		public IQuery NoMatchQuery
		{
			get { return _noMatchQuery; }
			set
			{
				_noMatchQuery = value;
				_noMatchQuerySet = true;
			}
		}

		public bool NoMatchFilterNone
		{
			get { return _noMatchFilterNone; }
			set
			{
				_noMatchFilterNone = value;
				_noMatchFilterNoneSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("indices");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteListValue("indices", _indices, elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("query");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_query.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			if (_noMatchQuerySet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("no_match_query");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_noMatchQuery.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			else if (!_noMatchQuerySet && _noMatchFilterNoneSet)
			{
				JsonHelper.WriteValue("no_match_query", "none", elasticsearchCrudJsonWriter);
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}