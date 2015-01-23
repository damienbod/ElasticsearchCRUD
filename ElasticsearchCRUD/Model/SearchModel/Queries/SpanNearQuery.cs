using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	/// <summary>
	/// Matches spans which are near one another. 
	/// One can specify slop, the maximum number of intervening unmatched positions, as well as whether matches are required to be in-order. 
	/// The span near query maps to Lucene SpanNearQuery. 
	/// 
	/// The clauses element is a list of one or more other span type queries and the slop controls the maximum number of intervening unmatched positions permitted.
	/// </summary>
	public class SpanNearQuery : ISpanQuery
	{
		private readonly List<SpanTermQuery> _queries;
		private readonly uint _slop;
		private bool _inOrder;
		private bool _inOrderSet;
		private bool _collectPayloads;
		private bool _collectPayloadsSet;

		public SpanNearQuery(List<SpanTermQuery> queries, uint slop)
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
			_slop = slop;
		}

		
		/// <summary>
		/// in_order
		/// </summary>
		public bool InOrder
		{
			get { return _inOrder; }
			set
			{
				_inOrder = value;
				_inOrderSet = true;
			}
		}

		/// <summary>
		/// collect_payloads
		/// </summary>
		public bool CollectPayloads
		{
			get { return _collectPayloads; }
			set
			{
				_collectPayloads = value;
				_collectPayloadsSet = true;
			}
		}
  

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("span_near");
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

			JsonHelper.WriteValue("slop", _slop, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("in_order", _inOrder, elasticsearchCrudJsonWriter, _inOrderSet);
			JsonHelper.WriteValue("collect_payloads", _collectPayloads, elasticsearchCrudJsonWriter, _collectPayloadsSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
