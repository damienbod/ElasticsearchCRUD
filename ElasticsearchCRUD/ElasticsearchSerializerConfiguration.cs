using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticsearchCRUD.Tracing;

namespace ElasticsearchCRUD
{
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

		public IElasticSearchMappingResolver ElasticSearchMappingResolver
		{
			get { return _elasticSearchMappingResolver; }
		}

		public bool SaveChildObjectsAsWellAsParent
		{
			get { return _saveChildObjectsAsWellAsParent; }
		}

		public bool ProcessChildDocumentsAsSeparateChildIndex
		{
			get { return _processChildDocumentsAsSeparateChildIndex; }
		}
	}
}
