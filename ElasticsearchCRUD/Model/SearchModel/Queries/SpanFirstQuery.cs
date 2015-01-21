using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Matches spans near the beginning of a field. The span first query maps to Lucene SpanFirstQuery.
	/// The match clause can be any other span type query. The end controls the maximum end position permitted in a match.
	/// </summary>
	public class SpanFirstQuery :IQuery
	{
		private readonly ISpanQuery _spanQuery;
		private readonly int _end;

		public SpanFirstQuery(ISpanQuery spanQuery, int end)
		{
			_spanQuery = spanQuery;
			_end = end;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("span_first");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("match");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			_spanQuery.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			JsonHelper.WriteValue("end", _end, elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
