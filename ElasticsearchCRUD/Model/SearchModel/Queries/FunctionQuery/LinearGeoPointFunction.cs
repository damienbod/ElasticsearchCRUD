using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.Units;

namespace ElasticsearchCRUD.Model.SearchModel.Queries.FunctionQuery
{
	public class LinearGeoPointFunction : GeoDecayBaseScoreFunction
	{
		public LinearGeoPointFunction(string field, GeoPoint origin, DistanceUnit scale) : base(field, origin, scale, "linear"){}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}
	}
}
