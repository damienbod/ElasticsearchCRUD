namespace ElasticsearchCRUD.Model.SearchModel.Aggregations.RangeParam
{
	public abstract class RangeAggregationParameter
	{
		protected string KeyValue;
		protected bool KeySet;

		public string Key
		{
			get { return KeyValue; }
			set
			{
				KeyValue = value;
				KeySet = true;
			}
		}

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}