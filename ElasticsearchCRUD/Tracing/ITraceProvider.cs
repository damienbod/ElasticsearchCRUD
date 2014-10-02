using System;
using System.Diagnostics;

namespace ElasticsearchCRUD.Tracing
{
	public interface ITraceProvider
	{
		void Trace(TraceEventType level, string message, params object[] args);
		void Trace(TraceEventType level, Exception ex, string message, params object[] args);
	}
}
