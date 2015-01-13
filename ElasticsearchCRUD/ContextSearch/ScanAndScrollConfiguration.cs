using System.Collections.Generic;
using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.ContextSearch
{
	public class ScanAndScrollConfiguration
	{
		private readonly TimeUnit _lengthOfTime;
		private readonly int _size = 50;

		public ScanAndScrollConfiguration(TimeUnit lengthOfTime, int size)
		{
			_lengthOfTime = lengthOfTime;
			_size = size;
		}

		public string GetScrollScanUrlForSetup()
		{
			return string.Format("search_type=scan&scroll={0}&size={1}", _lengthOfTime.GetTimeUnit(), _size);
		}

		public string GetScrollScanUrlForRunning()
		{
			return string.Format("_search/scroll?scroll={0}&scroll_id=", _lengthOfTime.GetTimeUnit());
		}

	}
}

