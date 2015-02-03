using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.ContextSearch.SearchModel.AggModel
{
	public class TermsAggregationsResult : AggregationResult<TermsAggregationsResult>
	{
		//doc_count_error_upper_bound":0,"sum_other_doc_count":0

		/// <summary>
		/// doc_count_error_upper_bound
		/// </summary>
		public int DocCountErrorUpperBound { get; set; }

		/// <summary>
		/// sum_other_doc_count
		/// </summary>
		public int SumOtherDocCount { get; set; }

		public List<TermBuckets> Buckets { get; set; }

		public override TermsAggregationsResult GetValueFromJToken(JToken result)
		{
			return result.ToObject<TermsAggregationsResult>();
		}
		
	}
}