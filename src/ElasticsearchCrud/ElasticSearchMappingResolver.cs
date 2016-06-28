using System;
using System.Collections.Generic;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// This class is used to add all register all the type definitions and then resolver them when requesting or handling response data from Elasticsearch.
	/// If no mapping is defined, the default mapping is used.
	/// </summary>
	public class ElasticsearchMappingResolver : IElasticsearchMappingResolver
	{
		private readonly Dictionary<Type, ElasticsearchMapping> _mappingDefinitions = new Dictionary<Type, ElasticsearchMapping>();

		public ElasticsearchMapping GetElasticSearchMapping(Type type)
		{
			if (_mappingDefinitions.ContainsKey(type))
			{
				return _mappingDefinitions[type];
			}

			_mappingDefinitions.Add(type, new ElasticsearchMapping());
			return _mappingDefinitions[type];
		}

		/// <summary>
		/// You can add custom Type handlers here for specific mapping.
		/// Only one mapping can be defined pro type.
		/// </summary>
		/// <param name="type">Type of class</param>
		/// <param name="mapping">mapping definition.</param>
		public void AddElasticSearchMappingForEntityType(Type type, ElasticsearchMapping mapping)
		{
			if (_mappingDefinitions.ContainsKey(type))
			{
				throw new ElasticsearchCrudException("The mapping for this type is already defined.");
			}
			_mappingDefinitions.Add(type, mapping);
		}
	}
}
