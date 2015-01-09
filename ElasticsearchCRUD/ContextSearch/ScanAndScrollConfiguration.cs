using System.Collections.Generic;
using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.ContextSearch
{
	public class ScanAndScrollConfiguration
	{
		private readonly Dictionary<ScanTimeUnits, string> _timeUnits = new Dictionary<ScanTimeUnits, string>();
		private readonly int _scroll = 1;
		private readonly ScanTimeUnits _timeUnitsForScroll = ScanTimeUnits.Minute;
		private readonly int _size = 50;

		public ScanAndScrollConfiguration(int scroll, ScanTimeUnits scrollTimeUnits, int size)
		{
			_timeUnits.Add(ScanTimeUnits.Year, "d");
			_timeUnits.Add(ScanTimeUnits.Month, "M");
			_timeUnits.Add(ScanTimeUnits.Week, "w");
			_timeUnits.Add(ScanTimeUnits.Day, "d");
			_timeUnits.Add(ScanTimeUnits.Hour, "h");
			_timeUnits.Add(ScanTimeUnits.Minute, "m");
			_timeUnits.Add(ScanTimeUnits.Second, "s");

			_scroll = scroll;
			_timeUnitsForScroll = scrollTimeUnits;
			_size = size;
		}

		public string GetScrollScanUrlForSetup()
		{
			return string.Format("search_type=scan&scroll={0}{1}&size={2}", _scroll, _timeUnits[_timeUnitsForScroll],_size);
		}

		public string GetScrollScanUrlForRunning()
		{
			return string.Format("_search/scroll?scroll={0}{1}&scroll_id=", _scroll, _timeUnits[_timeUnitsForScroll]);
		}

	}
}

