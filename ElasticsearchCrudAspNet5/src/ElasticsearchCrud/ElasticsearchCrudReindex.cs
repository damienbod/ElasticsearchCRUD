using System;
using System.Diagnostics;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.Units;
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

		/// <summary>
		/// Scan ans scroll settings for the reindex. You should keep this small, otherwise the response request will be too larger.
		/// Default is 5 seconds with a size of 500 documents pro shard.
		/// </summary>
		public ScanAndScrollConfiguration ScanAndScrollConfiguration = new ScanAndScrollConfiguration(new TimeUnitSecond(5), 500);

		private ITraceProvider _traceProvider = new NullTraceProvider();

		/// <summary>
		/// Reindex constructor. The class reuires a index and a type for the old index and also the new index. The old index can then be converteds or reindexed to the new index.
		/// </summary>
		/// <param name="oldIndexTypeDescription">index and index type parameters for the old index</param>
		/// <param name="newIndexTypeDescription">index and index type parameters for the new index</param>
		/// <param name="connectionString">Elasticsearch connection string</param>
		public ElasticsearchCrudReindex(IndexTypeDescription oldIndexTypeDescription, IndexTypeDescription newIndexTypeDescription, string connectionString)
		{
			_oldIndexTypeDescription = oldIndexTypeDescription;
			_newIndexTypeDescription = newIndexTypeDescription;
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(TNew), MappingUtils.GetElasticsearchMapping<TNew>(newIndexTypeDescription));
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(TOld), MappingUtils.GetElasticsearchMapping<TOld>(oldIndexTypeDescription));
			_context = new ElasticsearchContext(connectionString, elasticsearchMappingResolver);
		}

		/// <summary>
		/// TraceProvider if logging or tracing is used
		/// </summary>
		public ITraceProvider TraceProvider
		{
			get { return _traceProvider; }
			set
			{
				_context.TraceProvider = value;
				_traceProvider = value;
			}
		}

		/// <summary>
		/// Resets the alias from the old index to the new index. Assumes that a alias is used for the old indeex. This way, reindex can be done live.
		/// </summary>
		/// <param name="alias">alias string used for the index.</param>
		public void SwitchAliasfromOldToNewIndex(string alias)
		{
			_context.AliasReplaceIndex(alias, _oldIndexTypeDescription.Index, _newIndexTypeDescription.Index);
		}

		/// <summary>
		/// This method is used to reindex one index to a new index using a query, and two Functions.
		/// </summary>
		/// <param name="jsonContent">Json content for the search query</param>
		/// <param name="getKeyMethod">Func is require to define the _id required for the new index</param>
		/// <param name="convertMethod">Func used to map the old index to the new old, whatever your required mapping/conversion logic is</param>
		/// <param name="getRoutingDefinition">Function to get the RoutingDefinition of the document</param>
		public void Reindex(string jsonContent, Func<TOld, object> getKeyMethod, Func<TOld, TNew> convertMethod, Func<TOld, RoutingDefinition> getRoutingDefinition = null)
		{
			var result = _context.SearchCreateScanAndScroll<TOld>(jsonContent, ScanAndScrollConfiguration);

			var scrollId = result.PayloadResult.ScrollId;
			TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: Reindex: Total SearchResult in scan: {0}", result.PayloadResult.Hits.Total);

			int indexProccessed = 0;
			while (result.PayloadResult.Hits.Total > indexProccessed)
			{
				TraceProvider.Trace(TraceEventType.Information, "ElasticsearchCrudReindex: Reindex: creating new documents, indexProccessed: {0} SearchResult: {1}", indexProccessed, result.PayloadResult.Hits.Total);

				var resultCollection = _context.SearchScanAndScroll<TOld>(scrollId, ScanAndScrollConfiguration);
				scrollId = resultCollection.PayloadResult.ScrollId;

				foreach (var item in resultCollection.PayloadResult.Hits.HitsResult)
				{
					indexProccessed++;
					if (getRoutingDefinition != null)
					{
						_context.AddUpdateDocument(convertMethod(item.Source), getKeyMethod(item.Source), getRoutingDefinition(item.Source));
					}
					else
					{
						_context.AddUpdateDocument(convertMethod(item.Source), getKeyMethod(item.Source));
					}
					
				}
				_context.SaveChanges();
			}
		}
	}
}


