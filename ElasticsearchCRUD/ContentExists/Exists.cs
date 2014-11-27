using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContentExists
{
	public class Exists
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly HttpClient _client;

		public Exists(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, HttpClient client)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_client = client;
		}

		public async Task<ResultDetails<bool>> ExistsAsync(Uri uri)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "ExistsAsync: Request HEAD with url: {0}", uri.ToString());
			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Head, uri);
				var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(false);

				resultDetails.RequestUrl = uri.OriginalString;

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					resultDetails.PayloadResult = false;
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						throw new ElasticsearchCrudException("ExistsAsync: HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");
					}

					_traceProvider.Trace(TraceEventType.Information, "ExistsAsync:  response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase);
				}
				else
				{
					resultDetails.PayloadResult = true;
				}

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "ExistsAsync:  Get Request OperationCanceledException: {0}", oex.Message);
				return resultDetails;
			}
		}
	
	}
}
