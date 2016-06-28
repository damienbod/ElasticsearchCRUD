using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A multi-bucket value source based aggregation where buckets are dynamically built - one per unique value.
	/// </summary>
	public class TermsBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;
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
		private IncludeExpression _include;
		private bool _includeSet;
		private ExcludeExpression _exclude;
		private bool _excludeSet;
		private CollectMode _collectMode;
		private bool _collectModeSet;
		private ExecutionHint _executionHint;
		private bool _executionHintSet;
		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;

		public TermsBucketAggregation(string name, string field) : base("terms", name)
		{
			_field = field;
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

		public IncludeExpression Include
		{
			get { return _include; }
			set
			{
				_include = value;
				_includeSet = true;
			}
		}

		public ExcludeExpression Exclude
		{
			get { return _exclude; }
			set
			{
				_exclude = value;
				_excludeSet = true;
			}
		}

		/// <summary>
		/// Deferring calculation of child aggregations
		/// 
		/// For fields with many unique terms and a small number of required results it can be more efficient to delay the calculation of child aggregations 
		/// until the top parent-level aggs have been pruned. Ordinarily, all branches of the aggregation tree are expanded in one depth-first pass and only then any pruning occurs. 
		/// In some rare scenarios this can be very wasteful and can hit memory constraints.
		/// 
		/// When using breadth_first mode the set of documents that fall into the uppermost buckets are cached for subsequent replay so there is a memory overhead in doing this
		/// which is linear with the number of matching documents. 
		/// In most requests the volume of buckets generated is smaller than the number of documents that fall into them so the default depth_first collection mode is normally the best bet 
		/// but occasionally the breadth_first strategy can be significantly more efficient. 
		/// Currently elasticsearch will always use the depth_first collect_mode unless explicitly instructed to use breadth_first as in the above example. 
		/// Note that the order parameter can still be used to refer to data from a child aggregation when using the breadth_first setting 
		/// - the parent aggregation understands that this child aggregation will need to be called first before any of the other child aggregations.
		/// </summary>
		public CollectMode CollectMode
		{
			get { return _collectMode; }
			set
			{
				_collectMode = value;
				_collectModeSet = true;
			}
		}

		/// <summary>
		/// execution_hint
		/// There are different mechanisms by which terms aggregations can be executed:
		///
		/// by using field values directly in order to aggregate data per-bucket (map)
		/// by using ordinals of the field and preemptively allocating one bucket per ordinal value (global_ordinals)
		/// by using ordinals of the field and dynamically allocating one bucket per ordinal value (global_ordinals_hash)
		/// by using per-segment ordinals to compute counts and remap these counts to global counts using global ordinals (global_ordinals_low_cardinality) 
		///
		/// Elasticsearch tries to have sensible defaults so this is something that generally doesn’t need to be configured.
		/// </summary>
		public ExecutionHint ExecutionHint
		{
			get { return _executionHint; }
			set
			{
				_executionHint = value;
				_executionHintSet = true;
			}
		}

		public string Script
		{
			get { return _script; }
			set
			{
				_script = value;
				_scriptSet = true;
			}
		}

		public List<ScriptParameter> Params
		{
			get { return _params; }
			set
			{
				_params = value;
				_paramsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("field", _field, elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("size", _size, elasticsearchCrudJsonWriter, _sizeSet);
			JsonHelper.WriteValue("shard_size", _shardSize, elasticsearchCrudJsonWriter, _shardSizeSet);
			if (_orderSet)
			{
				_order.WriteJson(elasticsearchCrudJsonWriter);
			}
			JsonHelper.WriteValue("min_doc_count", _minDocCount, elasticsearchCrudJsonWriter, _minDocCountSet);
			JsonHelper.WriteValue("shard_min_doc_count", _shardMinDocCount, elasticsearchCrudJsonWriter, _shardMinDocCountSet);
			if (_includeSet)
			{
				_include.WriteJson(elasticsearchCrudJsonWriter);
			}
			if (_excludeSet)
			{
				_exclude.WriteJson(elasticsearchCrudJsonWriter);
			}
			JsonHelper.WriteValue("collect_mode", _collectMode.ToString(), elasticsearchCrudJsonWriter, _collectModeSet);
			JsonHelper.WriteValue("execution_hint", _executionHint.ToString(), elasticsearchCrudJsonWriter, _executionHintSet);

			if (_scriptSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("script");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
				if (_paramsSet)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("params");
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

					foreach (var item in _params)
					{
						elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
						elasticsearchCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
					}
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}
			}
		}
	}

	public enum ExecutionHint
	{
		/// <summary>
		/// should only be considered when very few documents match a query. Otherwise the ordinals-based execution modes are significantly faster. 
		/// By default, map is only used when running an aggregation on scripts, since they don’t have ordinals.
		/// </summary>
		map, 

		/// <summary>
		/// only works for leaf terms aggregations but is usually the fastest execution mode. 
		/// Memory usage is linear with the number of unique values in the field, so it is only enabled by default on low-cardinality fields.
		/// </summary>
		global_ordinals_low_cardinality,
		
		/// <summary>
		///  is the second fastest option, but the fact that it preemptively allocates buckets can be memory-intensive,
		///  especially if you have one or more sub aggregations. It is used by default on top-level terms aggregations.
		/// </summary>
		global_ordinals,

		/// <summary>
		///  on the contrary to global_ordinals and global_ordinals_low_cardinality allocates buckets dynamically 
		/// so memory usage is linear to the number of values of the documents that are part of the aggregation scope. It is used by default in inner aggregations.
		/// </summary>
		global_ordinals_hash
	}

	public enum CollectMode
	{
		breadth_first, depth_first
	}
}
