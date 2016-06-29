using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	using ElasticsearchCRUD.Model.SearchModel.Sorting;

	public class InnerHits
	{
		private int _from;
		private bool _fromSet;
		private int _size;
		private bool _sizeSet;
		private ISortHolder _sortHolder;
		private bool _sortHolderSet;
		private string _name;
		private bool _nameSet;

		/// <summary>
		/// name
		/// The name to be used for the particular inner hit definition in the response. Useful when multiple inner hits have been defined in a single search request. 
		/// The default depends in which query the inner hit is defined. For has_child query and filter this is the child type, 
		/// has_parent query and filter this is the parent type and the nested query and filter this is the nested path. 
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				_nameSet = true;
			}
		}

		/// <summary>
		/// How the inner hits should be sorted per inner_hits. By default the hits are sorted by the score.
		/// </summary>
		public ISortHolder Sort
		{
			get { return _sortHolder; }
			set
			{
				_sortHolder = value;
				_sortHolderSet = true;
			}
		}

		/// <summary>
		/// from
		/// The offset from where the first hit to fetch for each inner_hits in the returned regular search hits.
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
		/// The maximum number of hits to return per inner_hits. By default the top three matching hits are returned.
		/// </summary>
		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
				_sizeSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("inner_hits");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("from", _from, elasticsearchCrudJsonWriter, _fromSet);
			JsonHelper.WriteValue("size", _size, elasticsearchCrudJsonWriter, _sizeSet);

			if (_sortHolderSet)
			{
				_sortHolder.WriteJson(elasticsearchCrudJsonWriter);
			}
			JsonHelper.WriteValue("name", _name, elasticsearchCrudJsonWriter, _nameSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
