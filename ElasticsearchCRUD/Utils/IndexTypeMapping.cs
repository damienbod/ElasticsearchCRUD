using System;

namespace ElasticsearchCRUD.Utils
{
	public class IndexTypeMapping : ElasticsearchMapping
	{
		private readonly string _index;
		private readonly string _indexType;

		public IndexTypeMapping(string index, string indexType)
		{
			_index = index;
			_indexType = indexType;
		}

		public override string GetIndexForType(Type type)
		{
			return _index;
		}

		public override string GetDocumentType(Type type)
		{
			return _indexType;
		}
	}
}