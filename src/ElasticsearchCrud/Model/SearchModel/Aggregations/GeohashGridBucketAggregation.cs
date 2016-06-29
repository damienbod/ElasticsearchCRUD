using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Aggregations
{
	/// <summary>
	/// A multi-bucket aggregation that works on geo_point fields and groups points into buckets that represent cells in a grid. 
	/// The resulting grid can be sparse and only contains cells that have matching data. Each cell is labeled using a geohash which is of user-definable precision.
	/// 
	/// High precision geohashes have a long string length and represent cells that cover only a small area.
	/// 
	/// Low precision geohashes have a short string length and represent cells that each cover a large area. 
	/// 
	/// Geohashes used in this aggregation can have a choice of precision between 1 and 12.
	/// 
	/// Warning
	/// The highest-precision geohash of length 12 produces cells that cover less than a square metre of land and so high-precision requests can be very costly in terms of RAM 
	/// and result sizes. Please see the example below on how to first filter the aggregation to a smaller geographic area before requesting high-levels of detail.
	/// 
	/// The specified field must be of type geo_point (which can only be set explicitly in the mappings) and it can also hold an array of geo_point fields, 
	/// in which case all points will be taken into account during aggregation. 
	/// </summary>
	public class GeohashGridBucketAggregation : BaseBucketAggregation
	{
		private readonly string _field;

		private string _script;
		private List<ScriptParameter> _params;
		private bool _paramsSet;
		private bool _scriptSet;
		private uint _precision;
		private bool _precisionSet;
		private uint _size;
		private bool _sizeSet;
		private uint _shardSize;
		private bool _shardSizeSet;

		public GeohashGridBucketAggregation(string name, string field) : base("geohash_grid", name)
		{
			_field = field;
		}

		/// <summary>
		/// _precision The string length of the geohashes used to define cells/buckets in the results. Defaults to 5. 
		/// 
		/// 1 : 5,009.4km x 4,992.6km
		/// 2 : 1,252.3km x 624.1km
		/// 3 : 156.5km x 156km
		/// 4 : 39.1km x 19.5km
		/// 5 : 4.9km x 4.9km
		/// 6 : 1.2km x 609.4m
		/// 7 : 152.9m x 152.4m
		/// 8 : 38.2m x 19m
		/// 9 : 4.8m x 4.8m
		/// 10 : 1.2m x 59.5cm
		/// 11 : 14.9cm x 14.9cm
		/// 12 : 3.7cm x 1.9cm 
		/// </summary>
		public uint Precision
		{
			get { return _precision; }
			set
			{
				_precision = value;
				_precisionSet = true;
			}
		}

		/// <summary>
		/// The size parameter can be set to define how many term buckets should be returned out of the overall terms list. 
		/// By default, the node coordinating the search process will request each shard to provide its own top size term buckets and once all shards respond, 
		/// it will reduce the results to the final list that will then be returned to the client. This means that if the number of unique terms is greater than size, 
		/// the returned list is slightly off and not accurate 
		/// (it could be that the term counts are slightly off and it could even be that a term that should have been in the top size buckets was not returned). 
		/// If set to 0, the size will be set to Integer.MAX_VALUE.
		/// </summary>
		public uint Size
		{
			get { return _size; }
			set
			{
				_size = value;
				_sizeSet = true;
			}
		}

		/// <summary>
		/// shard_size
		/// The higher the requested size is, the more accurate the results will be, but also, the more expensive it will be to compute the final results 
		/// (both due to bigger priority queues that are managed on a shard level and due to bigger data transfers between the nodes and the client).
		/// 
		/// The shard_size parameter can be used to minimize the extra work that comes with bigger requested size. When defined, 
		/// it will determine how many terms the coordinating node will request from each shard. Once all the shards responded, 
		/// the coordinating node will then reduce them to a final result which will be based on the size parameter - this way, 
		/// one can increase the accuracy of the returned terms and avoid the overhead of streaming a big list of buckets back to the client. 
		/// If set to 0, the shard_size will be set to Integer.MAX_VALUE
		/// 
		/// Note
		/// shard_size cannot be smaller than size (as it doesn’t make much sense). When it is, elasticsearch will override it and reset it to be equal to size.
		/// It is possible to not limit the number of terms that are returned by setting size to 0. 
		/// Don’t use this on high-cardinality fields as this will kill both your CPU since terms need to be return sorted, and your network.
		/// </summary>
		public uint ShardSize
		{
			get { return _shardSize; }
			set
			{
				_shardSize = value;
				_shardSizeSet = true;
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
			JsonHelper.WriteValue("precision", _precision, elasticsearchCrudJsonWriter, _precisionSet);
			JsonHelper.WriteValue("size", _size, elasticsearchCrudJsonWriter, _sizeSet);
			JsonHelper.WriteValue("shard_size", _shardSize, elasticsearchCrudJsonWriter, _shardSizeSet);

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
}