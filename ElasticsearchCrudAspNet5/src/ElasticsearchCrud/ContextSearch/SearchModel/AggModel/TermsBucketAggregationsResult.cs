using System.Collections.Generic;
using ElasticsearchCRUD.ContextSearch.SearchModel.AggModel.Buckets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class TermsBucketAggregationsResult : AggregationResult<TermsBucketAggregationsResult>
	{
		//doc_count_error_upper_bound":0,"sum_other_doc_count":0

		/// <summary>
		/// doc_count_error_upper_bound
		/// </summary>
		[JsonProperty("doc_count_error_upper_bound")]
		public int DocCountErrorUpperBound { get; set; }

		/// <summary>
		/// sum_other_doc_count
		/// </summary>
		[JsonProperty("sum_other_doc_count")]
		public int SumOtherDocCount { get; set; }

		public List<Bucket> Buckets { get; set; }

		public override TermsBucketAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<TermsBucketAggregationsResult>();
		}
		
	}
}