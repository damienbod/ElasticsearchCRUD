using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD.ContextAlias
{
	class ElasticsearchContextAlias
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticsearchSerializerConfiguration _elasticsearchSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticsearchContextAlias(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticsearchSerializerConfiguration elasticsearchSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticsearchSerializerConfiguration = elasticsearchSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public bool CreateAliasForIndex(string alias, string index)
		{
			try
			{
				Task<ResultDetails<bool>> task = Task.Run(() => CreateAliasForIndexAsync(alias, index));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextAlias: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticsearchContextAlias: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextAlias: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticsearchContextAlias: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{0} Exception", "ElasticsearchContextAlias");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "ElasticsearchContextAlias: Creating Alias {1} for Index index {0}", index, alias);
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: Creating Alias {1} for Index index {0}", index, alias));
		}

		public async Task<ResultDetails<bool>> CreateAliasForIndexAsync(string alias, string index)
		{
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextAlias: Creating Alias {1} for Index index {0}", index, alias));

			if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				_traceProvider.Trace(TraceEventType.Error, "{1}: index is not allowed in Elasticsearch: {0}", index, "ElasticsearchContextAlias");
				throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: index is not allowed in Elasticsearch: {0}", index));
			}

			if (Regex.IsMatch(alias, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				_traceProvider.Trace(TraceEventType.Error, "{1}: alias is not allowed in Elasticsearch: {0}", alias, "ElasticsearchContextAlias");
				throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: index is not allowed in Elasticsearch: {0}", index));
			}

			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			var elasticsearchUrlForClearCache = string.Format("{0}/{1}/_cache/clear", _connectionString, index);
			var uri = new Uri(elasticsearchUrlForClearCache);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri, "ElasticsearchContextAlias");

			var content = new StringContent(BuildCreateAliasBody(alias, index));
			var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextAlias: Cound Not Create Alias {1} for Index index {0}", index, alias));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: Cound Not Create Alias {1} for Index index {0}", index, alias));
		}

		public bool RemoveAliasForIndex(string alias, string index)
		{
			try
			{
				Task<ResultDetails<bool>> task = Task.Run(() => RemoveAliasForIndexAsync(alias, index));
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextAlias: HttpStatusCode.NotFound");
					throw new ElasticsearchCrudException("ElasticsearchContextAlias: HttpStatusCode.NotFound");
				}
				if (task.Result.Status == HttpStatusCode.BadRequest)
				{
					_traceProvider.Trace(TraceEventType.Warning, "ElasticsearchContextAlias: HttpStatusCode.BadRequest");
					throw new ElasticsearchCrudException("ElasticsearchContextAlias: HttpStatusCode.BadRequest" + task.Result.Description);
				}
				return task.Result.PayloadResult;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "{0} Exception", "ElasticsearchContextAlias");
					if (x is ElasticsearchCrudException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticsearchCrudException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "ElasticsearchContextAlias: Remove Alias {1} for Index index {0}", index, alias);
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: Remove Alias {1} for Index index {0}", index, alias));
		}

		public async Task<ResultDetails<bool>> RemoveAliasForIndexAsync(string alias, string index)
		{
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextAlias: Remove Alias {1} for Index index {0}", index, alias));

			if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				_traceProvider.Trace(TraceEventType.Error, "{1}: index is not allowed in Elasticsearch: {0}", index, "ElasticsearchContextAlias");
				throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: index is not allowed in Elasticsearch: {0}", index));
			}

			if (Regex.IsMatch(alias, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				_traceProvider.Trace(TraceEventType.Error, "{1}: alias is not allowed in Elasticsearch: {0}", alias, "ElasticsearchContextAlias");
				throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: index is not allowed in Elasticsearch: {0}", index));
			}

			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			var elasticsearchUrlForClearCache = string.Format("{0}/{1}/_cache/clear", _connectionString, index);
			var uri = new Uri(elasticsearchUrlForClearCache);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri, "ElasticsearchContextAlias");

			var content = new StringContent(BuildRemoveAliasBody(alias, index));
			var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextAlias: Cound Not Create Alias {1} for Index index {0}", index, alias));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: Cound Not Create Alias {1} for Index index {0}", index, alias));
		}

		private string BuildCreateAliasBody(string alias, string index)
		{
			return "";
		}

		private string BuildRemoveAliasBody(string alias, string index)
		{
			return "";
		}
	}
}
