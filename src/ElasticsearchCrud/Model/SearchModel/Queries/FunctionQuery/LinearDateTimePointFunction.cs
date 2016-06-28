using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class LinearDateTimePointFunction : DateTimeDecayBaseScoreFunction
	{
		public LinearDateTimePointFunction(string field, TimeUnit scale) : base(field, scale, "linear") { }

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}