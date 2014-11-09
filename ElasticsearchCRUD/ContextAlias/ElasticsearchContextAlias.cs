using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

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

		public bool SendAliasCommand(string contentJson)
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.Execute(SendAliasCommandAsync(contentJson));	
		}

		public async Task<ResultDetails<bool>> SendAliasCommandAsync(string contentJson)
		{
			_traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticsearchContextAlias: Creating Alias {0}", contentJson));

			var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
			var elasticsearchUrlForClearCache = string.Format("{0}/_aliases", _connectionString);
			var uri = new Uri(elasticsearchUrlForClearCache);
			_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri, "ElasticsearchContextAlias");

			var content = new StringContent(contentJson);
			var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				resultDetails.PayloadResult = true;
				return resultDetails;
			}

			_traceProvider.Trace(TraceEventType.Error, string.Format("ElasticsearchContextAlias: Cound Not Execute Alias {0}", contentJson));
			throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: Cound Not Execute Alias  {0}", contentJson));
		}

		public void ValidateAlias(string alias)
		{
			if (Regex.IsMatch(alias, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				_traceProvider.Trace(TraceEventType.Error, "{1}: alias is not allowed in Elasticsearch: {0}", alias, "ElasticsearchContextAlias");
				throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: alias is not allowed in Elasticsearch: {0}", alias));
			}
		}

		public void ValidateIndex(string index)
		{
			if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				_traceProvider.Trace(TraceEventType.Error, "{1}: index is not allowed in Elasticsearch: {0}", index, "ElasticsearchContextAlias");
				throw new ElasticsearchCrudException(string.Format("ElasticsearchContextAlias: index is not allowed in Elasticsearch: {0}", index));
			}
		}

		//{
		//	"actions" : [
		//		{ "remove" : { "index" : "test1", "alias" : "alias1" } },
		//		{ "add" : { "index" : "test1", "alias" : "alias2" } }
		//	]
		//}'
		public string BuildCreateOrRemoveAlias(AliasAction action, string alias, string index)
		{
			ValidateAlias(alias);
			ValidateIndex(index);

			var sb = new StringBuilder();
			BuildAliasBegin(sb);
			BuildAction(sb, action, alias, index);
			BuildAliasEnd(sb);
			return sb.ToString();
		}

		public string BuildAliasChangeIndex(string alias, string indexOld, string indexNew)
		{
			ValidateAlias(alias);
			ValidateIndex(indexOld);
			ValidateIndex(indexNew);

			var sb = new StringBuilder();
			BuildAliasBegin(sb);
			BuildAction(sb, AliasAction.remove, alias, indexOld);
			sb.Append(",");
			BuildAction(sb, AliasAction.add, alias, indexNew);
			BuildAliasEnd(sb);
			return sb.ToString();
		}

		private void BuildAliasBegin(StringBuilder sb)
		{
			sb.AppendLine("{");
			sb.AppendLine("\"actions\" : [");
		}

		private void BuildAliasEnd(StringBuilder sb)
		{
			sb.AppendLine("]");
			sb.AppendLine("}");
		}

		private void BuildAction(StringBuilder sb, AliasAction action, string alias, string index)
		{
			sb.AppendLine("{ \"" + action + "\" : { \"index\" : \"" + index + "\", \"alias\" : \"" + alias + "\" } }");
		}

		
	}

	enum AliasAction
	{
		remove,
		add,
	}

}
