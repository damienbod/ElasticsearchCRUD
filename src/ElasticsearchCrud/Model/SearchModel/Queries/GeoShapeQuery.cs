using ElasticsearchCRUD.Model.GeoModel;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class GeoShapeQuery: IQuery
	{
		private readonly string _field;
		private readonly IGeoType _geoType;

		public GeoShapeQuery(string field, IGeoType geoType)
		{
			_field = field;
			_geoType = geoType;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geo_shape");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("shape");
		
			_geoType.WriteJson(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
