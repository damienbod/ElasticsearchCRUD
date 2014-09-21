using System;

namespace Damienbod.ElasticSearchProvider
{
	public interface ITraceProvider
	{
		void Trace(string message);
	}

	public class NullTraceProvider : ITraceProvider
	{
		public void Trace(string message)
		{
			// Do nothing as we do not want to trace messages per default
		}
	}

	public class ConsoleTraceProvider : ITraceProvider
	{
		public void Trace(string message)
		{
			Console.WriteLine(message);
		}
	}
}
