using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A multi-bucket values source based aggregation that can be applied on numeric values extracted from the documents. 
	/// It dynamically builds fixed size (a.k.a. interval) buckets over the values. For example, if the documents have a field that holds a price (numeric), 
	/// we can configure this aggregation to dynamically build buckets with interval 5 (in case of price it may represent $5). 
	/// When the aggregation executes, the price field of every document will be evaluated and will be rounded down to its closest bucket - for example, 
	/// if the price is 32 and the bucket size is 5 then the rounding will yield 30 and thus the document will "fall" into the bucket that is associated withe the key 30
	/// </summary>
	public class HistogramBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;
		private uint _minDocCount;
		private bool _minDocCountSet;
		private OrderAgg _order;
		private bool _orderSet;

		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;
		private readonly uint _interval;
		private ExtendedBounds _extendedBounds;
		private bool _extendedBoundsSet;
		private bool _keyed;
		private bool _keyedSet;

		public HistogramBucketAggregation(string name, string field, uint interval) : base("histogram", name)
		{
			_field = field;
			_interval = interval;
		}

		public OrderAgg Order
		{
			get { return _order; }
			set
			{
				_order = value;
				_orderSet = true;
			}
		}

		/// <summary>
		/// min_doc_count
		/// 
		/// Terms are collected and ordered on a shard level and merged with the terms collected from other shards in a second step. 
		/// However, the shard does not have the information about the global document count available. 
		/// The decision if a term is added to a candidate list depends only on the order computed on the shard using local shard frequencies. 
		/// The min_doc_count criterion is only applied after merging local terms statistics of all shards. 
		/// In a way the decision to add the term as a candidate is made without being very certain about if the term will actually reach the required min_doc_count. 
		/// This might cause many (globally) high frequent terms to be missing in the final result if low frequent terms populated the candidate lists. 
		/// To avoid this, the shard_size parameter can be increased to allow more candidate terms on the shards. However, this increases memory consumption and network traffic.
		/// </summary>
		public uint MinDocCount
		{
			get { return _minDocCount; }
			set
			{
				_minDocCount = value;
				_minDocCountSet = true;
			}
		}

		public ExtendedBounds ExtendedBounds
		{
			get { return _extendedBounds; }
			set
			{
				_extendedBounds = value;
				_extendedBoundsSet = true;
			}
		}

		/// <summary>
		/// If this value is set, the buckets are returned with id classes. 
		/// </summary>
		public bool Keyed
		{
			get { return _keyed; }
			set
			{
				_keyed = value;
				_keyedSet = true;
			}
		}

		public string Script
		{
			get { return _script; }
			set
			{
				_script = value;
				_scriptSet = true;
			}
		}

		public List<ScriptParameter> Params
		{
			get { return _params; }
			set
			{
				_params = value;
				_paramsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("field", _field, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("interval", _interval, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("keyed", _keyed, elasticsearchCrudJsonWriter, _keyedSet);
			if (_orderSet)
			{
				_order.WriteJson(elasticsearchCrudJsonWriter);
			}
			JsonHelper.WriteValue("min_doc_count", _minDocCount, elasticsearchCrudJsonWriter, _minDocCountSet);

			if (_extendedBoundsSet)
			{
				_extendedBounds.WriteJson(elasticsearchCrudJsonWriter);
			}

			if (_scriptSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("script");
				elasticsearchCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
				if (_paramsSet)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("params");
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

					foreach (var item in _params)
					{
						elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
						elasticsearchCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
					}
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}
			}
		}
	}

	public class ExtendedBounds
	{
		public uint Min { get; set; }
		public uint Max { get; set; }

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("extended_bounds");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("min", Min, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("max", Max, elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}