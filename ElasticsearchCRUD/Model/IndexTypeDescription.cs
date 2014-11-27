namespace ElasticsearchCRUD.Model
{
	public class IndexTypeDescription
	{
		private readonly string _index;
		private readonly string _indexType;

		public IndexTypeDescription(string index, string indexType)
		{
			_index = index;
			_indexType = indexType;
		}

		public string Index { get { return _index; } }
		public string IndexType { get { return _indexType; } }
	}
}