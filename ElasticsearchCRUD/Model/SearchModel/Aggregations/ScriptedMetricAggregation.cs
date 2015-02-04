

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class ScriptedMetricAggregation : IAggs
	{
		private readonly string _name;

		public ScriptedMetricAggregation(string name)
		{
			_name = name;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("scripted_metric");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			// TODOJsonHelper.WriteValue("field",_field,elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
