using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A multi-bucket value source based aggregation where buckets are dynamically built - one per unique value.
	/// </summary>
	public class TermsBucketAggregation : BaseBucketAggregation
	{
		private uint _size;
		private bool _sizeSet;
		private uint _shardSize;
		private bool _shardSizeSet;
		private OrderAgg _order;
		private bool _orderSet;
		private uint _minDocCount;
		private bool _minDocCountSet;
		private uint _shardMinDocCount;
		private bool _shardMinDocCountSet;

		public TermsBucketAggregation(string name, string field) : base("terms", name, field)
		{
		}

		/// <summary>
		/// The size parameter can be set to define how many term buckets should be returned out of the overall terms list. 
		/// By default, the node coordinating the search process will request each shard to provide its own top size term buckets and once all shards respond, 
		/// it will reduce the results to the final list that will then be returned to the client. This means that if the number of unique terms is greater than size, 
		/// the returned list is slightly off and not accurate 
		/// (it could be that the term counts are slightly off and it could even be that a term that should have been in the top size buckets was not returned). 
		/// If set to 0, the size will be set to Integer.MAX_VALUE.
		/// </summary>
		public uint Size
		{
			get { return _size; }
			set
			{
				_size = value;
				_sizeSet = true;
			}
		}

		/// <summary>
		/// shard_size
		/// The higher the requested size is, the more accurate the results will be, but also, the more expensive it will be to compute the final results 
		/// (both due to bigger priority queues that are managed on a shard level and due to bigger data transfers between the nodes and the client).
		/// 
		/// The shard_size parameter can be used to minimize the extra work that comes with bigger requested size. When defined, 
		/// it will determine how many terms the coordinating node will request from each shard. Once all the shards responded, 
		/// the coordinating node will then reduce them to a final result which will be based on the size parameter - this way, 
		/// one can increase the accuracy of the returned terms and avoid the overhead of streaming a big list of buckets back to the client. 
		/// If set to 0, the shard_size will be set to Integer.MAX_VALUE
		/// 
		/// Note
		/// shard_size cannot be smaller than size (as it doesn’t make much sense). When it is, elasticsearch will override it and reset it to be equal to size.
		/// It is possible to not limit the number of terms that are returned by setting size to 0. 
		/// Don’t use this on high-cardinality fields as this will kill both your CPU since terms need to be return sorted, and your network.
		/// </summary>
		public uint ShardSize
		{
			get { return _shardSize; }
			set
			{
				_shardSize = value;
				_shardSizeSet = true;
			}
		}
		
		public OrderAgg Order
		{
			get { return _order; }
			set
			{
				_order = value;
				_orderSet = true;
			}
		}

		/// <summary>
		/// min_doc_count
		/// 
		/// Terms are collected and ordered on a shard level and merged with the terms collected from other shards in a second step. 
		/// However, the shard does not have the information about the global document count available. 
		/// The decision if a term is added to a candidate list depends only on the order computed on the shard using local shard frequencies. 
		/// The min_doc_count criterion is only applied after merging local terms statistics of all shards. 
		/// In a way the decision to add the term as a candidate is made without being very certain about if the term will actually reach the required min_doc_count. 
		/// This might cause many (globally) high frequent terms to be missing in the final result if low frequent terms populated the candidate lists. 
		/// To avoid this, the shard_size parameter can be increased to allow more candidate terms on the shards. However, this increases memory consumption and network traffic.
		/// </summary>
		public uint MinDocCount
		{
			get { return _minDocCount; }
			set
			{
				_minDocCount = value;
				_minDocCountSet = true;
			}
		}

		/// <summary>
		/// shard_min_doc_count
		/// 
		/// The parameter shard_min_doc_count regulates the certainty a shard has if the term should actually be added to the candidate list or not with respect to the min_doc_count. 
		/// Terms will only be considered if their local shard frequency within the set is higher than the shard_min_doc_count. 
		/// If your dictionary contains many low frequent terms and you are not interested in those (for example misspellings), 
		/// then you can set the shard_min_doc_count parameter to filter out candidate terms on a shard level that will with a reasonable certainty not reach the required min_doc_count 
		/// even after merging the local counts. shard_min_doc_count is set to 0 per default and has no effect unless you explicitly set it.
		/// 
		/// Note
		/// 
		/// Setting min_doc_count=0 will also return buckets for terms that didn’t match any hit. 
		/// However, some of the returned terms which have a document count of zero might only belong to deleted documents, 
		/// so there is no warranty that a match_all query would find a positive document count for those terms
		/// 
		/// Warning
		/// 
		/// When NOT sorting on doc_count descending, high values of min_doc_count may return a number of buckets which is less than size because not enough data was gathered from the shards. 
		/// Missing buckets can be back by increasing shard_size. Setting shard_min_doc_count too high will cause terms to be filtered out on a shard level. 
		/// This value should be set much lower than min_doc_count/#shards.
		/// </summary>
		public uint ShardMinDocCount
		{
			get { return _shardMinDocCount; }
			set
			{
				_shardMinDocCount = value;
				_shardMinDocCountSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("size", _size, elasticsearchCrudJsonWriter, _sizeSet);
			JsonHelper.WriteValue("shard_size", _shardSize, elasticsearchCrudJsonWriter, _shardSizeSet);
			if (_orderSet)
			{
				_order.WriteJson(elasticsearchCrudJsonWriter);
			}
			JsonHelper.WriteValue("min_doc_count", _minDocCount, elasticsearchCrudJsonWriter, _minDocCountSet);
			JsonHelper.WriteValue("shard_min_doc_count", _shardMinDocCount, elasticsearchCrudJsonWriter, _shardMinDocCountSet);
		}
	}
}
