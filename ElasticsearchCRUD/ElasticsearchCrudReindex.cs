using System;
using System.Diagnostics;
using System.Text;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD
{
	public class ElasticsearchCrudReindex<TOld, TNew>
		where TNew : class
		where TOld : class
	{
		private readonly IndexTypeDescription _oldIndexTypeDescription;
		private readonly IndexTypeDescription _newIndexTypeDescription;
		private readonly int _indexerSize;
		private readonly ElasticsearchContext _context;

		public ScanAndScrollConfiguration ScanAndScrollConfiguration = new ScanAndScrollConfiguration(1, TimeUnits.Minute, 500);
		private ITraceProvider _traceProvider = new NullTraceProvider();

		public ElasticsearchCrudReindex(IndexTypeDescription oldIndexTypeDescription, IndexTypeDescription newIndexTypeDescription, string connectionString, int indexerSize)
		{
			_oldIndexTypeDescription = oldIndexTypeDescription;
			_newIndexTypeDescription = newIndexTypeDescription;
			_indexerSize = indexerSize;
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(TNew), MappingUtils.GetElasticsearchMapping(newIndexTypeDescription));
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(TOld), MappingUtils.GetElasticsearchMapping(oldIndexTypeDescription));
			_context = new ElasticsearchContext(connectionString, elasticsearchMappingResolver);
		}

		public ITraceProvider TraceProvider
		{
			get { return _traceProvider; }
			set
			{
				_context.TraceProvider = value;
				_traceProvider = value;
			}
		}
		public void SwitchAliasfromOldToNewIndex(string alias)
		{
			_context.AliasReplaceIndex(alias, _oldIndexTypeDescription.Index, _newIndexTypeDescription.Index);
		}

		public void Reindex(DateTime beginDateTime, Func<TOld, long> getKeyMethod, Func<TOld, TNew> convertMethod )
		{
			var result = _context.SearchCreateScanAndScroll<TOld>(BuildSearchModifiedDateTimeLessThan(beginDateTime), ScanAndScrollConfiguration);

			var scrollId = result.PayloadResult;
			TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: Reindex: Total Hits in scan: {0}", result.TotalHits);

			int indexPointer = 0;
			while (result.TotalHits > indexPointer - _indexerSize)
			{
				TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: Reindex: creating new documents, indexPointer: {0} Hits: {1}", indexPointer, result.TotalHits);

				var resultCollection = _context.Search<TOld>(BuildSearchModifiedDateTimeLessThan(beginDateTime, BuildSearchFromTooForScanScroll(indexPointer, _indexerSize)),
					scrollId);

				foreach (var item in resultCollection.PayloadResult)
				{
					_context.AddUpdateDocument(convertMethod(item), getKeyMethod(item));
				}
				_context.SaveChanges();
				indexPointer = indexPointer + _indexerSize;
			}
		}

		public void ReindexUpdateChangesWhileReindexing(DateTime beginDateTime, Func<TOld, long> getKeyMethod, Func<TOld, TNew> convertMethod)
		{
			var result = _context.SearchCreateScanAndScroll<TOld>(BuildSearchModifiedDateTimeGreaterThan(beginDateTime), ScanAndScrollConfiguration);
			var scrollId = result.PayloadResult;
			TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: ReindexUpdateChangesWhileReindexing: Total Hits in scan: {0}", result.TotalHits);

			int indexPointer = 0;
			while (result.TotalHits > indexPointer - _indexerSize)
			{
				TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: ReindexUpdateChangesWhileReindexing: creating new documents, indexPointer: {0} Hits: {1}", indexPointer, result.TotalHits);

				var resultCollection = _context.Search<TOld>(BuildSearchModifiedDateTimeGreaterThan(beginDateTime, BuildSearchFromTooForScanScroll(indexPointer, _indexerSize)),
					scrollId);

				foreach (var item in resultCollection.PayloadResult)
				{
					_context.AddUpdateDocument(convertMethod(item), getKeyMethod(item));
				}
				_context.SaveChanges();
				indexPointer = indexPointer + _indexerSize;
			}
		}
	
		//{   
		//   "from" : 100 , "size" : 100
		//}
		private string BuildSearchFromTooForScanScroll(int from, int size)
		{
			return "\"from\" : " + from + ", \"size\" : " + size + ",";
		}

		private string BuildSearchModifiedDateTimeLessThan(DateTime dateTimeUtc, string addFromSize = "")
		{
			return BuildSearchRange("lt", "modifieddate", dateTimeUtc, addFromSize);
		}

		private string BuildSearchModifiedDateTimeGreaterThan(DateTime dateTimeUtc, string addFromSize = "")
		{
			return BuildSearchRange("gte", "modifieddate", dateTimeUtc, addFromSize);
		}

		//{
		//   "query" :  {
		//	   "range": {  "modifieddate": { "lt":   "2003-12-29T00:00:00"  } }
		//	}
		//}
		private string BuildSearchRange(string lessThanOrGreaterThan, string updatePropertyName, DateTime dateTimeUtc, string addFromToSize)
		{
			string isoDateTime = dateTimeUtc.ToString("s");
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			if (!string.IsNullOrEmpty(addFromToSize))
			{
				buildJson.AppendLine(addFromToSize);
			}
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"range\": {  \"" + updatePropertyName + "\": { \"" + lessThanOrGreaterThan + "\":   \"" + isoDateTime + "\"  } }");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}
	}
}


