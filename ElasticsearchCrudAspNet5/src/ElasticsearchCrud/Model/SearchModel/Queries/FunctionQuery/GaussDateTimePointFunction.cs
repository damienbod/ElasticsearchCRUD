using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class GaussDateTimePointFunction : DateTimeDecayBaseScoreFunction
	{
		public GaussDateTimePointFunction(string field, TimeUnit scale) : base(field, scale, "gauss") { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}