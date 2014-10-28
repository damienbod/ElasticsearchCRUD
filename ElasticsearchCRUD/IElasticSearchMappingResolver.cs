using System;

namespace ElasticsearchCRUD
{
	public interface IElasticsearchMappingResolver
	{
		ElasticsearchMapping GetElasticSearchMapping(Type type);
		void AddElasticSearchMappingForEntityType(Type type, ElasticsearchMapping mapping);
	}
}