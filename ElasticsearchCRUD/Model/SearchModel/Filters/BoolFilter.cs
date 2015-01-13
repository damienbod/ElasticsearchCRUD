using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// A query that matches documents matching boolean combinations of other queries. The bool query maps to Lucene BooleanQuery. 
	/// It is built using one or more boolean clauses, each clause with a typed occurrence.
	///{
	///	"query":{
	///		"bool" : {
	///			"must" : [ 
	///				{
	///					"term" : { "details" : "different" }
	///				},
	///				{
	///					"term" : { "details" : "data" }
	///				}
	///			],
	///			"must_not" : [
	///				{
	///					"range" : {
	///						"id" : { "from" : 7, "to" : 20 }
	///					}
	///				}
	///			],
	///			"should" : [
	///				{
	///					"term" : { "details" : "data" }
	///				},
	///				{
	///					"term" : { "details" : "alone" }
	///				}
	///			]
	///		}
	///	}
	///}
	/// </summary>
	public class BoolFilter: IFilter
	{
		private List<IFilter> _must;
		private bool _mustSet;
		private List<IFilter> _mustNot;
		private bool _mustNotSet;
		private List<IFilter> _should;
		private bool _shouldSet;
		private bool _cache;
		private bool _cacheSet;

		public BoolFilter(){}

		public BoolFilter(IFilter must, IFilter mustNot = null)
		{
			Must = new List<IFilter> {must};

			if (mustNot != null)
			{
				MustNot = new List<IFilter> {mustNot};
			}
		}

		public List<IFilter> Must
		{
			get { return _must; }
			set
			{
				_must = value;
				_mustSet = true;
			}
		}

		public List<IFilter> MustNot
		{
			get { return _must; }
			set
			{
				_mustNot = value;
				_mustNotSet = true;
			}
		}

		public List<IFilter> Should
		{
			get { return _should; }
			set
			{
				_should = value;
				_shouldSet = true;
			}
		}

		public bool Cache
		{
			get { return _cache; }
			set
			{
				_cache = value;
				_cacheSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("bool");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteMustQueryList(elasticsearchCrudJsonWriter);
			WriteMustNotQueryList(elasticsearchCrudJsonWriter);
			WriteShouldQueryList(elasticsearchCrudJsonWriter);

			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void WriteShouldQueryList(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_shouldSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("should");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var shouldItem in _should)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					shouldItem.WriteJson(elasticsearchCrudJsonWriter);
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}

		private void WriteMustNotQueryList(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_mustNotSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("must_not");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var mustNotItem in _mustNot)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					mustNotItem.WriteJson(elasticsearchCrudJsonWriter);
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}

		private void WriteMustQueryList(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_mustSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("must");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var mustItem in _must)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					mustItem.WriteJson(elasticsearchCrudJsonWriter);
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
			}
		}
	}
}

