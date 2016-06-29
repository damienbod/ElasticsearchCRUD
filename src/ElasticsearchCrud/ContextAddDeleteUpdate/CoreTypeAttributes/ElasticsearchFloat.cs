using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ElasticsearchFloat : ElasticsearchNumber
	{
		public override string JsonString()
		{
			return JsonStringInternal("float");
		}
	}
}
