using System;
using System.Diagnostics;
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
		private readonly ElasticsearchContext _context;

		public ScanAndScrollConfiguration ScanAndScrollConfiguration = new ScanAndScrollConfiguration(5, TimeUnits.Second, 500);
		private ITraceProvider _traceProvider = new NullTraceProvider();

		public ElasticsearchCrudReindex(IndexTypeDescription oldIndexTypeDescription, IndexTypeDescription newIndexTypeDescription, string connectionString)
		{
			_oldIndexTypeDescription = oldIndexTypeDescription;
			_newIndexTypeDescription = newIndexTypeDescription;
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

		public void Reindex(string jsonContent, Func<TOld, object> getKeyMethod, Func<TOld, TNew> convertMethod)
		{
			var result = _context.SearchCreateScanAndScroll<TOld>(jsonContent, ScanAndScrollConfiguration);

			var scrollId = result.ScrollId;
			TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: Reindex: Total Hits in scan: {0}", result.TotalHits);

			int indexProccessed = 0;
			while (result.TotalHits > indexProccessed)
			{
				TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: Reindex: creating new documents, indexProccessed: {0} Hits: {1}", indexProccessed, result.TotalHits);

				var resultCollection = _context.Search<TOld>("",scrollId, ScanAndScrollConfiguration);
				scrollId = resultCollection.ScrollId;

				foreach (var item in resultCollection.PayloadResult)
				{
					indexProccessed++;
					_context.AddUpdateDocument(convertMethod(item), getKeyMethod(item));
				}
				_context.SaveChanges();
			}
		}
	}
}


