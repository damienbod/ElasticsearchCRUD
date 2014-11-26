namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class IndexSettings
	{
		private int _numberOfShards;
		private bool _numberOfShardsSet = true;
		private int _numberOfReplicas;
		private bool _numberOfReplicasSet = true;

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

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			WriteValue("number_of_shards", _numberOfShards, elasticsearchCrudJsonWriter, _numberOfShardsSet);
			WriteValue("number_of_replicas", _numberOfReplicas, elasticsearchCrudJsonWriter, _numberOfReplicasSet);
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
