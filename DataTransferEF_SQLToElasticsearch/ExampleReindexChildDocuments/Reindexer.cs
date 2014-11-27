using System;
using System.Diagnostics;
using DataTransferSQLToEl.SQLDomainModel;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;

namespace DataTransferSQLToEl.ExampleReindexChildDocuments
{
	public static class Reindexer
	{
		public static void ReindexStateProvince(DateTime beginDateTime)
		{
			var reindex = new ElasticsearchCrudReindex<StateProvince, StateProvinceV2>(
				new IndexTypeDescription("stateprovinces", "stateprovince"),
				new IndexTypeDescription("stateprovinces_v2", "stateprovince"),
				"http://localhost:9200");

			reindex.ScanAndScrollConfiguration = new ScanAndScrollConfiguration(5, TimeUnits.Second, 1000);
			reindex.TraceProvider = new ConsoleTraceProvider(TraceEventType.Information);

			reindex.Reindex(
				StateProvinceReindexConfiguration.BuildSearchModifiedDateTimeLessThan(beginDateTime),
				StateProvinceReindexConfiguration.GetKeyMethod,
				StateProvinceReindexConfiguration.CreateStateProvinceFromStateProvince);
		}

		public static void ReindexStateProvinceAddress(DateTime beginDateTime)
		{
			var reindexAddress = new ElasticsearchCrudReindex<Address, AddressV2>(
				new IndexTypeDescription("stateprovinces", "address"),
				new IndexTypeDescription("stateprovinces_v2", "address"),
				"http://localhost:9200");

			reindexAddress.ScanAndScrollConfiguration = new ScanAndScrollConfiguration(5, TimeUnits.Second, 1000);
			reindexAddress.TraceProvider = new ConsoleTraceProvider(TraceEventType.Information);

			reindexAddress.Reindex(
				StateProvinceReindexConfiguration.BuildSearchModifiedDateTimeLessThan(beginDateTime),
				AddressReindexConfiguration.GetKeyMethod,
				AddressReindexConfiguration.CreateStateProvinceFromStateProvince,
				AddressReindexConfiguration.GetRoutingConfiguration
				);
		}
	}
}
