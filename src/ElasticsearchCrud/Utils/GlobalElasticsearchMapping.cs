using System;

namespace ElasticsearchCRUD.Utils
{
	/// <summary>
	/// This mapping can be used if you require a search accross all indices and all types.
	/// </summary>
	public class GlobalElasticsearchMapping : ElasticsearchMapping
	{
		public override string GetIndexForType(Type type)
		{
			return "";
		}

		public override string GetDocumentType(Type type)
		{
			return "";
		}
	}
}