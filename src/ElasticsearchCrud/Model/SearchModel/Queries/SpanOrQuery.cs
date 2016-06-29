using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Matches the union of its span clauses. The span or query maps to Lucene SpanOrQuery.
	/// </summary>
	public class SpanOrQuery : ISpanQuery
	{
		private readonly List<ISpanQuery> _queries;

		public SpanOrQuery(List<ISpanQuery> queries )
		{
			if (queries == null)
			{
				throw new ElasticsearchCrudException("parameter List<ISpanQuery> queries cannot be null");
			}
			if (queries.Count < 0)
			{
				throw new ElasticsearchCrudException("parameter List<ISpanQuery> queries should have at least one element");
			}
			_queries = queries;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("span_or");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("clauses");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

			foreach (var item in _queries)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				item.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
			
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
