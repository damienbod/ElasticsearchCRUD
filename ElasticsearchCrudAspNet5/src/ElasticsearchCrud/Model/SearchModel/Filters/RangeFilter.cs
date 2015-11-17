using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class RangeFilter : IFilter
	{
		private readonly string _field;
		private object _greaterThanOrEqualTo;
		private bool _greaterThanOrEqualToSet;
		private object _greaterThan;
		private bool _greaterThanSet;
		private object _lessThanOrEqualTo;
		private bool _lessThanOrEqualToSet;
		private object _lessThan;
		private bool _lessThanSet;
		private string _timeZone;
		private bool _timeZoneSet;
		private bool _includeLower;
		private bool _includeLowerSet;
		private bool _includeUpper;
		private bool _includeUpperSet;

		public RangeFilter(string field)
		{
			_field = field;
		}

		/// <summary>
		/// gte
		/// Greater-than or equal to
		/// </summary>
		public object GreaterThanOrEqualTo
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
		public object GreaterThan
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
		public object LessThanOrEqualTo
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
		public object LessThan
		{
			get { return _lessThan; }
			set
			{
				_lessThan = value;
				_lessThanSet = true;
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

		/// <summary>
		/// time_zone
		/// When applied on date fields the range filter accepts also a time_zone parameter. 
		/// The time_zone parameter will be applied to your input lower and upper bounds and will move them to UTC time based date
		/// 
		/// if you give a date with a timezone explicitly defined and use the time_zone parameter, time_zone will be ignored. For example,
		/// setting gte to 2012-01-01T00:00:00+01:00 with "time_zone":"+10:00" will still use +01:00 time zone.
		/// </summary>
		public string TimeZone
		{
			get { return _timeZone; }
			set
			{
				_timeZone = value;
				_timeZoneSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("range");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(_field);
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("gte", _greaterThanOrEqualTo, elasticsearchCrudJsonWriter, _greaterThanOrEqualToSet);
			JsonHelper.WriteValue("gt", _greaterThan, elasticsearchCrudJsonWriter, _greaterThanSet);
			JsonHelper.WriteValue("lte", _lessThanOrEqualTo, elasticsearchCrudJsonWriter, _lessThanOrEqualToSet);
			JsonHelper.WriteValue("lt", _lessThan, elasticsearchCrudJsonWriter, _lessThanSet);
			JsonHelper.WriteValue("time_zone", _timeZone, elasticsearchCrudJsonWriter, _timeZoneSet);
			JsonHelper.WriteValue("include_lower", _includeLower, elasticsearchCrudJsonWriter, _includeLowerSet);
			JsonHelper.WriteValue("include_upper", _includeUpper, elasticsearchCrudJsonWriter, _includeUpperSet);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
