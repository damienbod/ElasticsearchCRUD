using ElasticsearchCRUD.Model.SearchModel.Sorting;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class OrderAgg
	{
		private readonly string _field;
		private readonly OrderEnum _order;

		public OrderAgg(string field, OrderEnum order)
		{
			_field = field;
			_order = order;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("order");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue(_field, _order.ToString(), elasticsearchCrudJsonWriter);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}