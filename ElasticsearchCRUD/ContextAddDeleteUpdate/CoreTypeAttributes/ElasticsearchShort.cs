using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ElasticsearchShort : ElasticsearchNumber
	{
		public override string JsonString()
		{
			return JsonStringInternal("short");
		}
	}
}