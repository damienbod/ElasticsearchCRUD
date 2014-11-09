using System;
using System.Text;
using DataTransferSQLToEl.SQLDomainModel;

namespace DataTransferSQLToEl
{
	internal class StateProvinceReindexConfiguration
	{
		public static string BuildSearchModifiedDateTimeLessThan(DateTime dateTimeUtc)
		{
			return BuildSearchRange("lt", "modifieddate", dateTimeUtc);
		}

		public static string BuildSearchModifiedDateTimeGreaterThan(DateTime dateTimeUtc)
		{
			return BuildSearchRange("gte", "modifieddate", dateTimeUtc);
		}

		//{
		//   "query" :  {
		//	   "range": {  "modifieddate": { "lt":   "2003-12-29T00:00:00"  } }
		//	}
		//}
		private static string BuildSearchRange(string lessThanOrGreaterThan, string updatePropertyName, DateTime dateTimeUtc)
		{
			string isoDateTime = dateTimeUtc.ToString("s");
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"range\": {  \"" + updatePropertyName + "\": { \"" + lessThanOrGreaterThan + "\":   \"" + isoDateTime + "\"  } }");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}
		public static object GetKeyMethod(StateProvince arg)
		{
			return arg.StateProvinceID;
		}

		public static StateProvinceV2 CreateStateProvinceFromStateProvince(StateProvince arg)
		{
			return new StateProvinceV2
			{
				StateProvinceID = arg.StateProvinceID,
				CountryRegionCode = arg.CountryRegionCode,
				IsOnlyStateProvinceFlag = arg.IsOnlyStateProvinceFlag,
				ModifiedDate = arg.ModifiedDate,
				Name = arg.Name,
				StateProvinceCode = arg.StateProvinceCode,
				rowguid = arg.rowguid
			};
		}
	}
}