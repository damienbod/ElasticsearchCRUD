using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ElasticsearchLong : ElasticsearchNumber
	{
		public override string JsonString()
		{
			return JsonStringInternal("long");
		}
	}
}