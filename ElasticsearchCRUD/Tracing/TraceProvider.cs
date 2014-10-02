using System;
using System.Diagnostics;
using System.Text;

namespace ElasticsearchCRUD.Tracing
{
	public class TraceProvider : ITraceProvider
	{
		public void Trace(TraceLevel level, string message, params object[] args)
		{
			WriteTrace(level, message, args);
		}

		public void Trace(TraceLevel level, Exception ex, string message, params object[] args)
		{
			var sb = new StringBuilder();
			sb.Append(string.Format(message, args));
			sb.AppendFormat("{2}: {0} , {1}", ex.Message, ex.StackTrace, ex.GetType());
			WriteTrace(level, sb.ToString());
		}

		private void WriteTrace(TraceLevel level, string message, params object[] args)
		{
			switch (level)
			{
				case TraceLevel.Error:
				{
					System.Diagnostics.Trace.TraceError(message, args);
					break;
				}
				case TraceLevel.Warning:
				{
					System.Diagnostics.Trace.TraceWarning(message, args);
					break;
				}
				case TraceLevel.Verbose:
				{
					System.Diagnostics.Trace.WriteLine(string.Format(message, args));
					break;
				}
				default:
				{
					System.Diagnostics.Trace.TraceInformation(message, args);
					break;
				}
			}
		}
	}
}