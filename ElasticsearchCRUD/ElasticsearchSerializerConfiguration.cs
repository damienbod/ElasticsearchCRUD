namespace ElasticsearchCRUD
{
	/// <summary>
	/// Configuration class used for the context settings.
	/// </summary>
	public class ElasticsearchSerializerConfiguration
	{
		private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver;
		private readonly bool _saveChildObjectsAsWellAsParent;
		private readonly bool _processChildDocumentsAsSeparateChildIndex;

		public ElasticsearchSerializerConfiguration(IElasticsearchMappingResolver elasticsearchMappingResolver, bool saveChildObjectsAsWellAsParent = true, bool processChildDocumentsAsSeparateChildIndex = false)
		{
			_elasticsearchMappingResolver = elasticsearchMappingResolver;
			_saveChildObjectsAsWellAsParent = saveChildObjectsAsWellAsParent;
			_processChildDocumentsAsSeparateChildIndex = processChildDocumentsAsSeparateChildIndex;
		}

		/// <summary>
		/// Mapping resolver used to get set each mapping configuration for a type. A type can only have one mapping pro context.
		/// </summary>
		public IElasticsearchMappingResolver ElasticsearchMappingResolver
		{
			get { return _elasticsearchMappingResolver; }
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
