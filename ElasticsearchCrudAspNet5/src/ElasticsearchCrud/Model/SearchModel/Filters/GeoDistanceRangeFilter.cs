using ElasticsearchCRUD.Model.GeoModel;
using ElasticsearchCRUD.Model.Units;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	/// <summary>
	/// Filters documents that include only hits that exists within a specific distance from a geo point. 
	/// </summary>
	public class GeoDistanceRangeFilter : IFilter
	{
		private readonly string _field;
		private readonly GeoPoint _location;
		private readonly DistanceUnit _from;
		private readonly DistanceUnit _to;
		private string _greaterThanOrEqualTo;
		private bool _greaterThanOrEqualToSet;
		private string _greaterThan;
		private bool _greaterThanSet;
		private string _lessThanOrEqualTo;
		private bool _lessThanOrEqualToSet;
		private string _lessThan;
		private bool _lessThanSet;
		private bool _cache;
		private bool _cacheSet;
		private bool _includeLower;
		private bool _includeLowerSet;
		private bool _includeUpper;
		private bool _includeUpperSet;

		/// <summary>
		/// Filters documents that include only hits that exists within a specific distance from a geo point. 
		/// </summary>
		/// <param name="field">name of the field used for the geo point</param>
		/// <param name="location">GeoPoint location</param>
		/// <param name="from">from in distance units</param>
		/// <param name="to">to in distance units</param>
		public GeoDistanceRangeFilter(string field, GeoPoint location, DistanceUnit from, DistanceUnit to)
		{
			_field = field;
			_location = location;
			_from = @from;
			_to = to;
		}

		/// <summary>
		/// gte
		/// Greater-than or equal to
		/// </summary>
		public string GreaterThanOrEqualTo
		{
			get { return _greaterThanOrEqualTo; }
			set
			{
				_greaterThanOrEqualTo = value;
				_greaterThanOrEqualToSet = true;
			}
		}

		/// <summary>
		/// gt
		/// Greater-than
		/// </summary>
		public string GreaterThan
		{
			get { return _greaterThan; }
			set
			{
				_greaterThan = value;
				_greaterThanSet = true;
			}
		}

		/// <summary>
		/// lte
		/// Less-than or equal to
		/// </summary>
		public string LessThanOrEqualTo
		{
			get { return _lessThanOrEqualTo; }
			set
			{
				_lessThanOrEqualTo = value;
				_lessThanOrEqualToSet = true;
			}
		}

		/// <summary>
		/// lt
		/// Less-than
		/// </summary>
		public string LessThan
		{
			get { return _lessThan; }
			set
			{
				_lessThan = value;
				_lessThanSet = true;
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

		/// <summary>
		/// include_lower
		/// </summary>
		public bool IncludeLower
		{
			get { return _includeLower; }
			set
			{
				_includeLower = value;
				_includeLowerSet = true;
			}
		}

		/// <summary>
		/// include_upper
		/// </summary>
		public bool IncludeUpper
		{
			get { return _includeUpper; }
			set
			{
				_includeUpper = value;
				_includeUpperSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("geo_distance_range");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			_location.WriteJson(elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("from", _from.GetDistanceUnit(), elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("to", _to.GetDistanceUnit(), elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("gte", _greaterThanOrEqualTo, elasticsearchCrudJsonWriter, _greaterThanOrEqualToSet);
			JsonHelper.WriteValue("gt", _greaterThan, elasticsearchCrudJsonWriter, _greaterThanSet);
			JsonHelper.WriteValue("lte", _lessThanOrEqualTo, elasticsearchCrudJsonWriter, _lessThanOrEqualToSet);
			JsonHelper.WriteValue("lt", _lessThan, elasticsearchCrudJsonWriter, _lessThanSet);
			JsonHelper.WriteValue("_cache", _cache, elasticsearchCrudJsonWriter, _cacheSet);
			JsonHelper.WriteValue("include_lower", _includeLower, elasticsearchCrudJsonWriter, _includeLowerSet);
			JsonHelper.WriteValue("include_upper", _includeUpper, elasticsearchCrudJsonWriter, _includeUpperSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}

}
