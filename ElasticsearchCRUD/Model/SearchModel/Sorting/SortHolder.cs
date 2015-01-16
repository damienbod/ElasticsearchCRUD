using System.Collections.Generic;

namespace ElasticsearchCRUD.Model.SearchModel.Sorting
{
	/// <summary>
	/// Allows to add one or more sort on specific fields. Each sort can be reversed as well. 
	/// The sort is defined on a per field level, with special field name for _score to sort by score.
	/// 
	/// When sorting, the relevant sorted field values are loaded into memory. This means that per shard, there should be enough memory to contain them. 
	/// For string based types, the field sorted on should not be analyzed / tokenized. For numeric types, if possible, 
	/// it is recommended to explicitly set the type to six_hun types (like short, integer and float).
	/// </summary>
	public class SortHolder : ISortHolder
	{
		private readonly List<ISort> _sortItems;

		public SortHolder(List<ISort> sortItems)
		{
			if (sortItems == null)
			{
				throw new ElasticsearchCrudException("SortHolder requires some sort values");
			}
			_sortItems = sortItems;
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("sort");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

			foreach (var item in _sortItems)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				item.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
		}
	}

	public interface ISortHolder
	{
		void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);
	}
}