using System.Collections.Generic;
using ElasticsearchCRUD.Model.SearchModel.Sorting;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel
{
	/// <summary>
	/// The search request can be executed with a search DSL, which includes the Query DSL, within its body.
	/// </summary>
	public class Search
	{
		private IQueryHolder _query;
		private bool _querySet;
		private IFilterHolder _filter;
		private bool _filterSet;
		private TimeUnit _timeout;
		private bool _timeoutSet;
		private int _from;
		private bool _fromSet;
		private int _size;
		private bool _sizeSet;
		private int _terminateAfter;
		private bool _terminateAfterSet;
		private bool _sortSet;
		private ISortHolder _sortHolder;
		private List<IAggs> _aggs;
		private bool _aggsSet;
		private Highlight _highlight;
		private bool _highlightSet;
		private List<Rescore> _rescore;
		private bool _rescoreSet;

		/// <summary>
		/// timeout
		/// A search timeout, bounding the search request to be executed within the specified time value and bail with the hits accumulated up to that point when expired. 
		/// Defaults to no timeout. See the section called “Time unitsedit”. 
		/// </summary>
		public TimeUnit Timeout
		{
			get { return _timeout; }
			set
			{
				_timeout = value;
				_timeoutSet = true;
			}
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

		/// <summary>
		/// terminate_after
		/// [1.4.0.Beta1] Added in 1.4.0.Beta1. The maximum number of documents to collect for each shard, upon reaching which the query execution will terminate early. 
		/// If set, the response will have a boolean field terminated_early to indicate whether the query execution has actually terminated_early. Defaults to no terminate_after. 
		/// </summary>
		public int TerminateAfter
		{
			get { return _terminateAfter; }
			set
			{
				_terminateAfter = value;
				_terminateAfterSet = true;
			}
		}

		public IQueryHolder Query
		{
			get { return _query; }
			set
			{
				_query = value;
				_querySet = true;
			}
		}

		public IFilterHolder Filter
		{
			get { return _filter; }
			set
			{
				_filter = value;
				_filterSet = true;
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

		public Highlight Highlight
		{
			get { return _highlight; }
			set
			{
				_highlight = value;
				_highlightSet = true;
			}
		}

		public List<Rescore> Rescore
		{
			get { return _rescore; }
			set
			{
				_rescore = value;
				_rescoreSet = true;
			}
		}

		/// <summary>
		/// aggregations request
		/// </summary>
		public List<IAggs> Aggs
		{
			get { return _aggs; }
			set
			{
				_aggs = value;
				_aggsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			if (_timeout != null)
			{
				JsonHelper.WriteValue("timeout", _timeout.GetTimeUnit(), elasticsearchCrudJsonWriter, _timeoutSet);
			}			
			JsonHelper.WriteValue("from", _from, elasticsearchCrudJsonWriter, _fromSet);
			JsonHelper.WriteValue("size", _size, elasticsearchCrudJsonWriter, _sizeSet);
			JsonHelper.WriteValue("terminate_after", _terminateAfter, elasticsearchCrudJsonWriter, _terminateAfterSet);

			if (_querySet)
			{
				_query.WriteJson(elasticsearchCrudJsonWriter);
			}

			if (_filterSet)
			{
				_filter.WriteJson(elasticsearchCrudJsonWriter);
			}

			if (_sortSet)
			{
				_sortHolder.WriteJson(elasticsearchCrudJsonWriter);
			}

			if (_aggsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				foreach (var item in _aggs)
				{
					item.WriteJson(elasticsearchCrudJsonWriter);
				}
				
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();	
			}

			if (_highlightSet)
			{
				_highlight.WriteJson(elasticsearchCrudJsonWriter);
			}

			if (_rescoreSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("rescore");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var rescore in _rescore)
				{
					rescore.WriteJson(elasticsearchCrudJsonWriter);
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}			

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		public override string ToString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			WriteJson(elasticsearchCrudJsonWriter);
			return elasticsearchCrudJsonWriter.GetJsonString();
		}
	}
}
