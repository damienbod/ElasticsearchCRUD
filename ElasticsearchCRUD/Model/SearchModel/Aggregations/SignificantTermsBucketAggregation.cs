using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// Example use cases:
	///
	///	Suggesting "H5N1" when users search for "bird flu" in text
	///	Identifying the merchant that is the "common point of compromise" from the transaction history of credit card owners reporting loss
	///	Suggesting keywords relating to stock symbol $ATI for an automated news classifier
	///	Spotting the fraudulent doctor who is diagnosing more than his fair share of whiplash injuries
	///	Spotting the tire manufacturer who has a disproportionate number of blow-outs 
	///
	/// In all these cases the terms being selected are not simply the most popular terms in a set. 
	/// They are the terms that have undergone a significant change in popularity measured between a foreground and background set. 
	/// If the term "H5N1" only exists in 5 documents in a 10 million document index and yet is found in 4 of the 100 documents that make up a user’s search results that is significant
	///  and probably very relevant to their search. 5/10,000,000 vs 4/100 is a big swing in frequency.
	/// Single-set analysise
	///
	/// In the simplest case, the foreground set of interest is the search results matched by a query and the background set used for statistical comparisons is the index 
	/// or indices from which the results were gathered.
	///
	/// </summary>
	public class SignificantTermsBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;
		private uint _size;
		private bool _sizeSet;
		private uint _shardSize;
		private bool _shardSizeSet;
		private uint _minDocCount;
		private bool _minDocCountSet;
		private uint _shardMinDocCount;
		private bool _shardMinDocCountSet;
		private IncludeExpression _include;
		private bool _includeSet;
		private ExcludeExpression _exclude;
		private bool _excludeSet;
		private ExecutionHint _executionHint;
		private bool _executionHintSet;
		private IFilter _backgroundFilter;
		private bool _backgroundFilterSet;
		private InformationRetrieval _informationRetrieval;
		private bool _informationRetrievalSet;

		public SignificantTermsBucketAggregation(string name, string field)
			: base("significant_terms", name)
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

		public IFilter BackgroundFilter
		{
			get { return _backgroundFilter; }
			set
			{
				_backgroundFilter = value;
				_backgroundFilterSet = true;
			}
		}

		/// <summary>
		/// mutual_information, chi_square, gnd, jlh
		/// 
		/// Mutual information as described in "Information Retrieval", Manning et al., Chapter 13.5.1 can be used as significance score by adding the parameter
		///
        /// "mutual_information": {
        ///      "include_negatives": true
        ///  }
		///
		/// Mutual information does not differentiate between terms that are descriptive for the subset or for documents outside the subset. 
		/// The significant terms therefore can contain terms that appear more or less frequent in the subset than outside the subset. 
		/// To filter out the terms that appear less often in the subset than in documents outside the subset, include_negatives can be set to false.
		/// 
		/// Per default, the assumption is that the documents in the bucket are also contained in the background. 
		/// If instead you defined a custom background filter that represents a different set of documents that you want to compare to, set
		/// 
		/// Roughly, mutual_information prefers high frequent terms even if they occur also frequently in the background. For example, 
		/// in an analysis of natural language text this might lead to selection of stop words. mutual_information is unlikely to select very rare terms like misspellings. 
		/// gnd prefers terms with a high co-occurence and avoids selection of stopwords. It might be better suited for synonym detection. 
		/// However, gnd has a tendency to select very rare terms that are, for example, a result of misspelling. chi_square and jlh are somewhat in-between.
		/// 
		/// It is hard to say which one of the different heuristics will be the best choice as it depends on what the significant terms are used for 
		/// (see for example [Yang and Pedersen, "A Comparative Study on Feature Selection in Text Categorization", 1997]
		/// (http://courses.ischool.berkeley.edu/i256/f06/papers/yang97comparative.pdf) 
		/// for a study on using significant terms for feature selection for text classification).
		/// </summary>
		public InformationRetrieval InformationRetrievalRetrieval
		{
			get { return _informationRetrieval; }
			set
			{
				_informationRetrieval = value;
				_informationRetrievalSet = true;
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
			
			JsonHelper.WriteValue("execution_hint", _executionHint.ToString(), elasticsearchCrudJsonWriter, _executionHintSet);

			if (_backgroundFilterSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("background_filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_backgroundFilter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			if (_informationRetrievalSet)
			{
				_informationRetrieval.WriteJson(elasticsearchCrudJsonWriter);
			}
		}
	}

	/// <summary>
	/// mutual_information
	/// </summary>
	public abstract class InformationRetrieval
	{	
		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}

	public class Jlh : InformationRetrieval
	{
		
		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(InformationRetrievalEnum.jlh.ToString());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
;
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public class Gnd : InformationRetrieval
	{
		private bool _backgroundIsSuperset;
		private bool _backgroundIsSupersetSet;

		public bool BackgroundIsSuperset
		{
			get { return _backgroundIsSuperset; }
			set
			{
				_backgroundIsSuperset = value;
				_backgroundIsSupersetSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(InformationRetrievalEnum.gnd.ToString());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("background_is_superset", _backgroundIsSuperset, elasticsearchCrudJsonWriter, _backgroundIsSupersetSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public class ChiSquare : InformationRetrieval
	{
		private bool _includeNegatives;
		private bool _includeNegativesSet;
		private bool _backgroundIsSuperset;
		private bool _backgroundIsSupersetSet;

		public bool IncludeNegatives
		{
			get { return _includeNegatives; }
			set
			{
				_includeNegatives = value;
				_includeNegativesSet = true;
			}
		}

		public bool BackgroundIsSuperset
		{
			get { return _backgroundIsSuperset; }
			set
			{
				_backgroundIsSuperset = value;
				_backgroundIsSupersetSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(InformationRetrievalEnum.chi_square.ToString());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("include_negatives", _includeNegatives, elasticsearchCrudJsonWriter, _includeNegativesSet);
			JsonHelper.WriteValue("background_is_superset", _backgroundIsSuperset, elasticsearchCrudJsonWriter, _backgroundIsSupersetSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public class MutualInformation : InformationRetrieval
	{
		private bool _includeNegatives;
		private bool _includeNegativesSet;
		private bool _backgroundIsSuperset;
		private bool _backgroundIsSupersetSet;

		public bool IncludeNegatives
		{
			get { return _includeNegatives; }
			set
			{
				_includeNegatives = value;
				_includeNegativesSet = true;
			}
		}

		public bool BackgroundIsSuperset
		{
			get { return _backgroundIsSuperset; }
			set
			{
				_backgroundIsSuperset = value;
				_backgroundIsSupersetSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(InformationRetrievalEnum.mutual_information.ToString());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("include_negatives", _includeNegatives, elasticsearchCrudJsonWriter, _includeNegativesSet);
			JsonHelper.WriteValue("background_is_superset", _backgroundIsSuperset, elasticsearchCrudJsonWriter, _backgroundIsSupersetSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public enum InformationRetrievalEnum
	{
		mutual_information, chi_square, gnd, jlh
	}
}