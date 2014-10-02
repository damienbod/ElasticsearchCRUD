using System;
using System.Diagnostics;

namespace ElasticsearchCRUD.Tracing
{
	public class ConsoleTraceProvider : ITraceProvider
	{
		public void Trace(TraceEventType level, string message, params object[] args)
		{
			SetForegroundColor(level);
			Console.WriteLine(level + ": " + message, args);
		}

		public void Trace(TraceEventType level, Exception ex, string message, params object[] args)
		{
			SetForegroundColor(level);
			Console.WriteLine(level + ": " + ex.Message + ex.InnerException + message, args);
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