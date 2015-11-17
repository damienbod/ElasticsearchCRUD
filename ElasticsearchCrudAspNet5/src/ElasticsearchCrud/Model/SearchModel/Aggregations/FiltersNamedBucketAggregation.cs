using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class FiltersNamedBucketAggregation : BaseBucketAggregation
	{
		private readonly List<NamedFilter> _namedFilters;

		public FiltersNamedBucketAggregation(string name, List<NamedFilter> namedFilters)
			: base("filters", name)
		{
			_namedFilters = namedFilters;
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filters");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			foreach (var filter in _namedFilters)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(filter.Name);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				filter.Filter.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

	public class NamedFilter
	{
		public IFilter Filter { get; private set; }
		public string Name { get; private set; }

		public NamedFilter(string name, IFilter filter)
		{
			Name = name;
			Filter = filter;
		}
	}
}