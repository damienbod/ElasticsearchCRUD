using System;

namespace ElasticsearchCRUD.Utils
{
	public class IndexMapping : ElasticsearchMapping
	{
		private readonly string _index;

		public IndexMapping(string index)
		{
			_index = index;
		}

		public override string GetIndexForType(Type type)
		{
			return _index;
		}
	}
}