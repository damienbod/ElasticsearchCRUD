namespace ElasticsearchCRUD.Utils
{
	public static class MappingUtils
	{
		public static ElasticsearchMapping GetElasticsearchMapping<T>(IndexTypeDescription indexTypeDescription)
		{
			return new IndexTypeMapping(indexTypeDescription.Index, indexTypeDescription.IndexType, typeof(T));
		}

		public static ElasticsearchMapping GetElasticsearchMapping<T>(string index, string indexType)
		{
			return new IndexTypeMapping(index, indexType, typeof(T));
		}

		public static ElasticsearchMapping GetElasticsearchMapping(string index)
		{
			return new IndexMapping(index);
		}
	}
}
