namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// The span_multi query allows you to wrap a multi term query (one of fuzzy, prefix, term range or regexp query) as a span query, so it can be nested. Example:
	/// </summary>
	public class SpanMultiQuery : ISpanQuery
	{
		private readonly IQuery _query;

		public SpanMultiQuery(FuzzyQuery query)
		{
			_query = query;
		}

		public SpanMultiQuery(PrefixQuery query)
		{
			_query = query;
		}

		public SpanMultiQuery(RegExpQuery query)
		{
			_query = query;
		}

		public SpanMultiQuery(RangeQuery query)
		{
			_query = query;
		}

		public SpanMultiQuery(WildcardQuery query)
		{
			_query = query;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("span_multi");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("match");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_query.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}