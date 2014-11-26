namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class OptimizeParameters
	{
		// TODO
		// max_num_segments
		// The number of segments to optimize to. To fully optimize the index, set it to 1. Defaults to simply checking if a merge needs to execute, and if so, executes it.

		//only_expunge_deletes
		//Should the optimize process only expunge segments with deletes in it. In Lucene, a document is not deleted from a segment, just marked as deleted. During a merge process of segments, a new segment is created that does not have those deletes. This flag allows to only merge segments that have deletes. Defaults to false. Note that this won’t override the index.merge.policy.expunge_deletes_allowed threshold.

		//flush
		//Should a flush be performed after the optimize. Defaults to true.

		//wait_for_merge
		//Should the request wait for the merge to end. Defaults to true. Note, a merge can potentially be a very heavy operation, so it might make sense to run it set to false. 

		public string GetOptimizeParameters()
		{
			return "";
		}
	}
}