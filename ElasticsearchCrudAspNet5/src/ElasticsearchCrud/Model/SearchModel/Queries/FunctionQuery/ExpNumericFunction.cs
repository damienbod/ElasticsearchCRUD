namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class ExpNumericFunction<T> : DecayBaseScoreFunction<T>
	{
		public ExpNumericFunction(string field, T origin, T scale) : base(field, origin, scale, "exp"){}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}