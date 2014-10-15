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
		private readonly bool _includeChildObjectsInDocument;
		private readonly bool _processChildDocumentsAsSeparateChildIndex;

		public ElasticsearchSerializerConfiguration(IElasticSearchMappingResolver elasticSearchMappingResolver, bool includeChildObjectsInDocument = true, bool processChildDocumentsAsSeparateChildIndex = false)
		{
			_elasticSearchMappingResolver = elasticSearchMappingResolver;
			_includeChildObjectsInDocument = includeChildObjectsInDocument;
			_processChildDocumentsAsSeparateChildIndex = processChildDocumentsAsSeparateChildIndex;
		}

		public IElasticSearchMappingResolver ElasticSearchMappingResolver
		{
			get { return _elasticSearchMappingResolver; }
		}

		public bool IncludeChildObjectsInDocument
		{
			get { return _includeChildObjectsInDocument; }
		}

		public bool ProcessChildDocumentsAsSeparateChildIndex
		{
			get { return _processChildDocumentsAsSeparateChildIndex; }
		}
	}
}
