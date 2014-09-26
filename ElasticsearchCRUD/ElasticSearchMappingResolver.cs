using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD
{
	public class ElasticSearchMappingResolver : IElasticSearchMappingResolver
	{
		private readonly Dictionary<Type, ElasticSearchMapping> _mappingDefinitions = new Dictionary<Type, ElasticSearchMapping>();

		public ElasticSearchMapping GetElasticSearchMapping(Type type)
		{
			if (_mappingDefinitions.ContainsKey(type))
			{
				return _mappingDefinitions[type];
			}

			_mappingDefinitions.Add(type, new ElasticSearchMapping());
			return _mappingDefinitions[type];
		}

		public void AddElasticSearchMappingForEntityType(Type type, ElasticSearchMapping mapping)
		{
			if (_mappingDefinitions.ContainsKey(type))
			{
				throw new ElasticsearchCrudException("The mapping for this type is already defined.");
			}
			_mappingDefinitions.Add(type, mapping);
		}
	}
}
