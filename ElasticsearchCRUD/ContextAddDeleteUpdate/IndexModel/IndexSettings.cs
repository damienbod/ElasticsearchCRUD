namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class IndexSettings
	{
		private int _numberOfShards;
		private bool _numberOfShardsSet = true;
		private int _numberOfReplicas;
		private bool _numberOfReplicasSet = true;

		private string _refreshInterval;
		private bool _refreshIntervalSet = true;
		

		public int NumberOfShards
		{
			get { return _numberOfShards; }
			set
			{
				_numberOfShards = value;
				_numberOfShardsSet = true;
			}
		}

		/// <summary>
		/// index.number_of_replicas
		///	The number of replicas each shard has. 
		/// </summary>
		public int NumberOfReplicas
		{
			get { return _numberOfReplicas; }
			set
			{
				_numberOfReplicas = value;
				_numberOfReplicasSet = true;
			}
		}


		 
		/// <summary>
		/// bulk Indexing Usage 
		/// For example, the update settings API can be used to dynamically change the index from being more performant for bulk indexing, 
		/// and then move it to more real time indexing state. Before the bulk indexing is started, use:
		///  "refresh_interval" : "-1"
		/// (Another optimization option is to start the index without any replicas, and only later adding them, but that really depends on the use case).
		/// Then, once bulk indexing is done, the settings can be updated (back to the defaults for example):
		/// "refresh_interval" : "1s"
		/// And, an optimize should be called:
		/// curl -XPOST 'http://localhost:9200/test/_optimize?max_num_segments=5'
		/// </summary>
		public string RefreshInterval
		{
			get { return _refreshInterval; }
			set
			{
				_refreshInterval = value;
				_refreshIntervalSet = true;
			}
		}

//index.auto_expand_replicas (string)
//	Set to a dash delimited lower and upper bound (e.g. 0-5) or one may use all as the upper bound (e.g. 0-all), or false to disable it. 
//index.blocks.read_only
//	Set to true to have the index read only, false to allow writes and metadata changes. 
//index.blocks.read
//	Set to true to disable read operations against the index. 
//index.blocks.write
//	Set to true to disable write operations against the index. 
//index.blocks.metadata
//	Set to true to disable metadata operations against the index. 
//index.refresh_interval
//	The async refresh interval of a shard. 
//index.index_concurrency
//	Defaults to 8. 
//index.codec.bloom.load
//	Whether to load the bloom filter. Defaults to false. 
//index.fail_on_merge_failure
//	Default to true. 
//index.translog.flush_threshold_ops
//	When to flush based on operations. 
//index.translog.flush_threshold_size
//	When to flush based on translog (bytes) size. 
//index.translog.flush_threshold_period
//	When to flush based on a period of not flushing. 
//index.translog.disable_flush
//	Disables flushing. Note, should be set for a short interval and then enabled. 
//index.cache.filter.max_size
//	The maximum size of filter cache (per segment in shard). Set to -1 to disable. 
//index.cache.filter.expire
//	The expire after access time for filter cache. Set to -1 to disable. 
//index.gateway.snapshot_interval
//	The gateway snapshot interval (only applies to shared gateways). Defaults to 10s. 
//merge policy
//	All the settings for the merge policy currently configured. A different merge policy can’t be set. 
//index.routing.allocation.include.*
//	A node matching any rule will be allowed to host shards from the index. 
//index.routing.allocation.exclude.*
//	A node matching any rule will NOT be allowed to host shards from the index. 
//index.routing.allocation.require.*
//	Only nodes matching all rules will be allowed to host shards from the index. 
//index.routing.allocation.disable_allocation
//	Disable allocation. Defaults to false. Deprecated in favour for index.routing.allocation.enable. 
//index.routing.allocation.disable_new_allocation
//	Disable new allocation. Defaults to false. Deprecated in favour for index.routing.allocation.enable. 
//index.routing.allocation.disable_replica_allocation
//	Disable replica allocation. Defaults to false. Deprecated in favour for index.routing.allocation.enable. 
//index.routing.allocation.enable

//	Enables shard allocation for a specific index. It can be set to:

//		all (default) - Allows shard allocation for all shards.
//		primaries - Allows shard allocation only for primary shards.
//		new_primaries - Allows shard allocation only for primary shards for new indices.
//		none - No shard allocation is allowed. 

//index.routing.allocation.total_shards_per_node
//	Controls the total number of shards (replicas and primaries) allowed to be allocated on a single node. Defaults to unbounded (-1). 
//index.recovery.initial_shards

//	When using local gateway a particular shard is recovered only if there can be allocated quorum shards in the cluster. It can be set to:

//		quorum (default)
//		quorum-1 (or half)
//		full
//		full-1.
//		Number values are also supported, e.g. 1. 

//index.gc_deletes , index.ttl.disable_purge
//	Disables temporarily the purge of expired docs. 
//store level throttling
//	All the settings for the store level throttling policy currently configured. 
//index.translog.fs.type
//	Either simple or buffered (default). 
//index.compound_format
//	See index.compound_format in the section called “Index Settingsedit”. 
//index.compound_on_flush
//	See `index.compound_on_flush in the section called “Index Settingsedit”. 
//Index Slow Log
//	All the settings for slow log. 
//index.warmer.enabled
//	See Warmers. Defaults to true. 
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteValue("number_of_shards", _numberOfShards, elasticsearchCrudJsonWriter, _numberOfShardsSet);
			WriteValue("number_of_replicas", _numberOfReplicas, elasticsearchCrudJsonWriter, _numberOfReplicasSet);
			WriteValue("refresh_interval", _refreshInterval, elasticsearchCrudJsonWriter, _refreshIntervalSet);
			
		}

		private void WriteValue(string key, object valueObj, ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, bool writeValue = true)
		{
			if (writeValue)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(key);
				elasticsearchCrudJsonWriter.JsonWriter.WriteValue(valueObj);
			}
		}
	}
}
