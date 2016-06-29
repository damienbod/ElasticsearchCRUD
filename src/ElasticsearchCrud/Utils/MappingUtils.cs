using System.Text.RegularExpressions;
using ElasticsearchCRUD.Model;

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

		public static void GuardAgainstBadIndexName(string index)
		{
			if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
			{
				throw new ElasticsearchCrudException(string.Format("ElasticsearchCrudJsonWriter: index is not allowed in Elasticsearch: {0}", index));
			}
		}
	}
}
