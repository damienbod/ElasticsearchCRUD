using System;

namespace ElasticsearchCRUD
{
	public interface IElasticSearchMappingResolver
	{
		ElasticSearchMapping GetElasticSearchMapping(Type type);
		void AddElasticSearchMappingForEntityType(Type type, ElasticSearchMapping mapping);
	}
}