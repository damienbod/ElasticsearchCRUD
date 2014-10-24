using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD
{
	/// <summary>
	/// Configuration class used for the context settings.
	/// </summary>
	public class ElasticsearchSerializerConfiguration
	{
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver;
		private readonly bool _saveChildObjectsAsWellAsParent;
		private readonly bool _processChildDocumentsAsSeparateChildIndex;

		public ElasticsearchSerializerConfiguration(IElasticSearchMappingResolver elasticSearchMappingResolver, bool saveChildObjectsAsWellAsParent = true, bool processChildDocumentsAsSeparateChildIndex = false)
		{
			_elasticSearchMappingResolver = elasticSearchMappingResolver;
			_saveChildObjectsAsWellAsParent = saveChildObjectsAsWellAsParent;
			_processChildDocumentsAsSeparateChildIndex = processChildDocumentsAsSeparateChildIndex;
		}

		/// <summary>
		/// Mapping resolver used to get set each mapping configuration for a type. A type can only have one mapping pro context.
		/// </summary>
		public IElasticSearchMappingResolver ElasticSearchMappingResolver
		{
			get { return _elasticSearchMappingResolver; }
		}

		/// <summary>
		/// Saves all child objects as well as the parent if set. 
		/// The child objects will be saved as nested or as separate documents depending on ProcessChildDocumentsAsSeparateChildIndex
		/// </summary>
		public bool SaveChildObjectsAsWellAsParent
		{
			get { return _saveChildObjectsAsWellAsParent; }
		}

		/// <summary>
		/// Context will save child objects as separate types in the same index if set. Otherwise child itemas are saved as nested objects.
		/// </summary>
		public bool ProcessChildDocumentsAsSeparateChildIndex
		{
			get { return _processChildDocumentsAsSeparateChildIndex; }
		}
	}
}
