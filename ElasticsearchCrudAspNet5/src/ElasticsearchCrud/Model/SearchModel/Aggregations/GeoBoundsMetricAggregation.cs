using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A metric aggregation that computes the bounding box containing all geo_point values for a field.
	/// </summary>
	public class GeoBoundsMetricAggregation : BaseMetricAggregation
	{
		private bool _wrapLongitude;
		private bool _wrapLongitudeSet;
		public GeoBoundsMetricAggregation(string name, string field) : base("geo_bounds", name, field) { }

		public bool WrapLongitude
		{
			get { return _wrapLongitude; }
			set
			{
				_wrapLongitude = value;
				_wrapLongitudeSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("wrap_longitude", _wrapLongitude, elasticsearchCrudJsonWriter, _wrapLongitudeSet);
		}
	}
}