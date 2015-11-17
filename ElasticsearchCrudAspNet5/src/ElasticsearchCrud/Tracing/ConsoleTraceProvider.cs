using System;
using System.Diagnostics;

namespace ElasticsearchCRUD.Tracing
{
	public class ConsoleTraceProvider : ITraceProvider
	{
		private readonly TraceEventType _traceEventTypelogLevel;

		public ConsoleTraceProvider()
		{
			_traceEventTypelogLevel = TraceEventType.Verbose;
		}

		public ConsoleTraceProvider(TraceEventType traceEventTypelogLevel)
		{
			_traceEventTypelogLevel = traceEventTypelogLevel;
		}

		public void Trace(TraceEventType level, string message, params object[] args)
		{
			if (_traceEventTypelogLevel >= level)
			{
				SetForegroundColor(level);
				Console.WriteLine(level + ": " + message, args);
			}
		}

		public void Trace(TraceEventType level, Exception ex, string message, params object[] args)
		{
			if (_traceEventTypelogLevel >= level)
			{
				SetForegroundColor(level);
				Console.WriteLine(level + ": " + ex.Message + ex.InnerException + message, args);
			}
		}

		private void SetForegroundColor(TraceEventType level)
		{
			switch (level)
			{
				case TraceEventType.Critical:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				}
				case TraceEventType.Error:
				{
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				}
				case TraceEventType.Warning:
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				}
				case TraceEventType.Verbose:
				{
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				}
				default:
				{
					Console.ForegroundColor = ConsoleColor.White;
					break;
				}
			}
		}
	}
}