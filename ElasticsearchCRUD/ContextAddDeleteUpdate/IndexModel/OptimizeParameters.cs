using System.Text;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel
{
	public class OptimizeParameters
	{
		private int _maxNumSegments;
		private bool _maxNumSegmentsSet;

		private bool _onlyExpungeDeletes;
		private bool _onlyExpungeDeletesSet;
		private bool _flush;
		private bool _flushSet;
		private bool _waitForMerge;
		private bool _waitForMergeSet;

		// max_num_segments
		// The number of segments to optimize to. To fully optimize the index, set it to 1. Defaults to simply checking if a merge needs to execute, and if so, executes it.
		public int NumberOfShards
		{
			get { return _maxNumSegments; }
			set
			{
				_maxNumSegments = value;
				_maxNumSegmentsSet = true;
			}
		}

		//only_expunge_deletes
		//Should the optimize process only expunge segments with deletes in it. In Lucene, a document is not deleted from a segment, just marked as deleted. 
		// During a merge process of segments, a new segment is created that does not have those deletes. 
		// This flag allows to only merge segments that have deletes. Defaults to false. 
		// Note that this won’t override the index.merge.policy.expunge_deletes_allowed threshold.
		public bool OnlyExpungeDeletesSet
		{
			get { return _onlyExpungeDeletes; }
			set
			{
				_onlyExpungeDeletes = value;
				_onlyExpungeDeletesSet = true;
			}
		}

		//flush
		//Should a flush be performed after the optimize. Defaults to true.
		public bool Flush
		{
			get { return _flush; }
			set
			{
				_flush = value;
				_flushSet = true;
			}
		}

		//wait_for_merge
		//Should the request wait for the merge to end. Defaults to true. Note, a merge can potentially be a very heavy operation, so it might make sense to run it set to false. 
		public bool WaitForMerge
		{
			get { return _waitForMerge; }
			set
			{
				_waitForMerge = value;
				_waitForMergeSet = true;
			}
		}
		
		public string GetOptimizeParameters()
		{
			var sb = new StringBuilder();

			bool firstParam = WriteValue("max_num_segments", _maxNumSegments, sb, true, _maxNumSegmentsSet);
			firstParam = WriteValue("only_expunge_deletes", _onlyExpungeDeletes.ToString().ToLower(), sb, firstParam, _onlyExpungeDeletesSet);
			firstParam = WriteValue("flush", _flush.ToString().ToLower(), sb, firstParam, _flushSet);
			WriteValue("wait_for_merge", _waitForMerge.ToString().ToLower(), sb, firstParam, _waitForMergeSet);
			
			return sb.ToString();
		}

		private bool WriteValue(string key, object data, StringBuilder sb, bool firstParam, bool writeValue = true)
		{
			if (!writeValue)
			{
				return firstParam;
			}
			sb.Append(firstParam ? "?" : "&");

			sb.Append(key);
			sb.Append("=");
			sb.Append(data);

			return false;
		}
	}
}