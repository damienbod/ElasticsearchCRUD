using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class ExpGeoPointFunction : GeoDecayBaseScoreFunction
	{
		public ExpGeoPointFunction(string field, GeoPoint origin, DistanceUnit scale) : base(field, origin, scale, "exp"){}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}