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

		public int NumberOfReplicas
		{
			get { return _numberOfReplicas; }
			set
			{
				_numberOfReplicas = value;
				_numberOfReplicasSet = true;
			}
		}

		// bulk Indexing Usage 
		// For example, the update settings API can be used to dynamically change the index from being more performant for bulk indexing, 
		// and then move it to more real time indexing state. Before the bulk indexing is started, use:
		//  "refresh_interval" : "-1"
		// (Another optimization option is to start the index without any replicas, and only later adding them, but that really depends on the use case).
		// Then, once bulk indexing is done, the settings can be updated (back to the defaults for example):
		// "refresh_interval" : "1s"
		// And, an optimize should be called:
		// curl -XPOST 'http://localhost:9200/test/_optimize?max_num_segments=5'
		public string RefreshInterval
		{
			get { return _refreshInterval; }
			set
			{
				_refreshInterval = value;
				_refreshIntervalSet = true;
			}
		}
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
