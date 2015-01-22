
using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class SpanNotQuery : ISpanQuery
	{
		private readonly ISpanQuery _include;
		private readonly ISpanQuery _exclude;

		/// <summary>
		/// Removes matches which overlap with another span query. The span not query maps to Lucene SpanNotQuery.
		/// The include and exclude clauses can be any span type query. The include clause is the span query whose matches are filtered, 
		/// and the exclude clause is the span query whose matches must not overlap those returned.
		/// 
		/// In the above example all documents with the term hoya are filtered except the ones that have la preceeding them.
		/// </summary>
		public SpanNotQuery(ISpanQuery include, ISpanQuery exclude)
		{	
			_include = include;
			_exclude = exclude;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("span_not");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("include");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_include.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("exclude");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			_exclude.WriteJson(elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
