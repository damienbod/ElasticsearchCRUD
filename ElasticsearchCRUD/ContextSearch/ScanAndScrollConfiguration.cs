using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchCRUD.ContextSearch
{
	public class ScanAndScrollConfiguration
	{
		private readonly Dictionary<TimeUnits, string> _timeUnits = new Dictionary<TimeUnits, string>();
		private readonly int _scroll = 1;
		private readonly TimeUnits _timeUnitsForScroll = TimeUnits.Minute;
		private readonly int _size = 50;

		public ScanAndScrollConfiguration(int scroll, TimeUnits scrollTimeUnits, int size)
		{
			_timeUnits.Add(TimeUnits.Year, "d");
			_timeUnits.Add(TimeUnits.Month, "M");
			_timeUnits.Add(TimeUnits.Week, "w");
			_timeUnits.Add(TimeUnits.Day, "d");
			_timeUnits.Add(TimeUnits.Hour, "h");
			_timeUnits.Add(TimeUnits.Minute, "m");
			_timeUnits.Add(TimeUnits.Second, "s");

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

