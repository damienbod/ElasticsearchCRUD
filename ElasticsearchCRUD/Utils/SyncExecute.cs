using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.Utils
{
	public class SyncExecute
	{
		private readonly ITraceProvider _traceProvider;

		public SyncExecute(ITraceProvider traceProvider)
		{
			_traceProvider = traceProvider;
		}

		public T Execute<T>(Task<ResultDetails<T>> method)
		{
			try
			{
				Task<ResultDetails<T>> task = Task.Run(() => method);
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Information, "SyncExecute: Exists HttpStatusCode.NotFound");
				}

				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "SyncExecute: Exists {0}", typeof(T));
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "SyncExecute: Unknown error for Exists  Type {0}", typeof(T));
			throw new ElasticsearchCrudException(string.Format("SyncExecute: Unknown error for Exists Type {0}", typeof(T)));
		}
	}
}
