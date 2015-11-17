namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class GaussNumericFunction<T> : DecayBaseScoreFunction<T>
	{
		public GaussNumericFunction(string field, T origin, T scale) : base(field, origin, scale, "gauss") { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}