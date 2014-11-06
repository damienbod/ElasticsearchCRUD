namespace ElasticsearchCRUD.Utils
{
	public static class MappingUtils
	{
		public static ElasticsearchMapping GetElasticsearchMapping(IndexTypeDescription indexTypeDescription)
		{
			return new IndexTypeMapping(indexTypeDescription.Index, indexTypeDescription.IndexType);
		}

		public static ElasticsearchMapping GetElasticsearchMapping(string index, string indexType)
		{
			return new IndexTypeMapping(index, indexType);
		}

		public static ElasticsearchMapping GetElasticsearchMapping(string index)
		{
			return new IndexMapping(index);
		}
	}
}
