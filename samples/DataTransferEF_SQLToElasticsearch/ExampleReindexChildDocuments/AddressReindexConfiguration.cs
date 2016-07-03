using DataTransferSQLToEl.SQLDomainModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;

namespace DataTransferSQLToEl
{
	internal class AddressReindexConfiguration
	{
		public static object GetKeyMethod(Address arg)
		{
			return arg.AddressID;
		}

		public static RoutingDefinition GetRoutingConfiguration(Address arg)
		{
			return new RoutingDefinition { ParentId = arg.StateProvinceID };
		}

		public static AddressV2 CreateStateProvinceFromStateProvince(Address arg)
		{
			return new AddressV2
			{
				AddressID = arg.AddressID,
				AddressLine1 = arg.AddressLine1,
				AddressLine2 = arg.AddressLine2,
				City = arg.City,
				ModifiedDate = arg.ModifiedDate,
				PostalCode = arg.PostalCode,
				rowguid = arg.rowguid
			};
		}
	}
}