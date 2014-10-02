using System;
using System.Diagnostics;

namespace ElasticsearchCRUD.Tracing
{
	public class NullTraceProvider : ITraceProvider
	{
		public void Trace(TraceLevel level, string message, params object[] args) { }
		public void Trace(TraceLevel level, Exception ex, string message, params object[] args) { }

	}
}