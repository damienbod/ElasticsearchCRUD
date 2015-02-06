using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A multi-bucket value source based aggregation where buckets are dynamically built - one per unique value.
	/// </summary>
	public class FiltersBucketAggregation : BaseBucketAggregation
	{
		private readonly List<IFilter> _filters;

		public FiltersBucketAggregation(string name, List<IFilter> filters)
			: base("filters", name)
		{
			_filters = filters;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filters");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

			foreach (var filter in _filters)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				filter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();			
		}
	}
}
