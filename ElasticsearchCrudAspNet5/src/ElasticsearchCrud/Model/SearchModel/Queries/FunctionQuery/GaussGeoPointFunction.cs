using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class GaussGeoPointFunction : GeoDecayBaseScoreFunction
	{
		public GaussGeoPointFunction(string field, GeoPoint origin, DistanceUnit scale) : base(field, origin, scale, "gauss"){}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}