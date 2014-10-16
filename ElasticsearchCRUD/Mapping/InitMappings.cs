using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using Newtonsoft.Json.Linq;

namespace ElasticsearchCRUD.Mapping
{
	public class InitMappings
	{
		public List<MappingCommand> Commands = new List<MappingCommand>(); 

		public class MappingCommand
		{
			public string RequestType { get; set; }

			public string Url { get; set; }

			public string Content { get; set; }
		}

		public async Task<ResultDetails<string>> Execute(HttpClient client, ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource)
		{
			var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };
			foreach (var command in Commands)
			{
				var content = new StringContent(command.Content);
				traceProvider.Trace(TraceEventType.Verbose, "{1}: sending bulk request: {0}", command, "InitMappings");
				traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", command.Url, "InitMappings");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				HttpResponseMessage response;
				if (command.RequestType == "POST")
				{
					response = await client.PostAsync(command.Url, content, cancellationTokenSource.Token).ConfigureAwait(true);
				}
				else
				{
					response = await client.PutAsync(command.Url, content, cancellationTokenSource.Token).ConfigureAwait(true);
				}
				
				//resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					traceProvider.Trace(TraceEventType.Warning, "{2}: SaveChangesAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "InitMappings");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						resultDetails.Description = errorInfo;
						return resultDetails;
					}

					return resultDetails;
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				var responseObject = JObject.Parse(responseString);
				traceProvider.Trace(TraceEventType.Verbose, "{1}: response: {0}", responseString, "InitMappings");
			}

			// no errors
			resultDetails.Status = HttpStatusCode.OK;
			return resultDetails;
		}
	}
}
