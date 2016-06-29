using ElasticsearchCRUD.Model.SearchModel.Sorting;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	public class TopHitsMetricAggregation : IAggs
	{
		private readonly string _name;
		private int _from;
		private bool _fromSet;
		private int _size;
		private bool _sizeSet;
		private ISortHolder _sortHolder;
		private bool _sortSet;

		public TopHitsMetricAggregation(string name)
		{
			_name = name;
		}

		/// <summary>
		/// from
		/// The starting from index of the hits to return. Defaults to 0. 
		/// </summary>
		public int From
		{
			get { return _from; }
			set
			{
				_from = value;
				_fromSet = true;
			}
		}

		/// <summary>
		/// size
		/// The number of hits to return. Defaults to 10. 
		/// </summary>
		public int Size
		{
			get { return _size; }
			set
			{
				_size = value;
				_sizeSet = true;
			}
		}

		public ISortHolder Sort
		{
			get { return _sortHolder; }
			set
			{
				_sortHolder = value;
				_sortSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_name);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("top_hits");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("from", _from, elasticsearchCrudJsonWriter, _fromSet);
			JsonHelper.WriteValue("size", _size, elasticsearchCrudJsonWriter, _sizeSet);
			if (_sortSet)
			{
				_sortHolder.WriteJson(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}