using System;
using System.Diagnostics;

namespace ElasticsearchCRUD.Tracing
{
	public class ConsoleTraceProvider : ITraceProvider
	{
		public void Trace(TraceLevel level, string message, params object[] args)
		{
			SetForegroundColor(level);
			Console.WriteLine(level + ": " + message, args);
		}

		public void Trace(TraceLevel level, Exception ex, string message, params object[] args)
		{
			SetForegroundColor(level);
			Console.WriteLine(level + ": " + ex.Message + ex.InnerException + message, args);
		}

		private void SetForegroundColor(TraceLevel level)
		{
			switch (level)
			{
				case TraceLevel.Error:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				}
				case TraceLevel.Warning:
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				}
				case TraceLevel.Verbose:
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