using System;

namespace ElasticsearchCRUD.Model
{
	public class DateTimeFormats
	{
		/// <summary>
		/// A basic formatter for a full date as four digit year, two digit month of year, and two digit day of month (yyyyMMdd).
		/// </summary>
		public const string BasicDate = "basic_date";

		/// <summary>
		/// A basic formatter that combines a basic date and time, separated by a T (yyyyMMdd’T'HHmmss.SSSZ).
		/// </summary>
		public const string BasicDateTime = "basic_date_time";

		/// <summary>
		/// A basic formatter that combines a basic date and time without millis, separated by a T (yyyyMMdd’T'HHmmssZ).
		/// </summary>
		public const string BasicDateTimeNoMillis = "basic_date_time_no_millis";

		/// <summary>
		/// A formatter for a full ordinal date, using a four digit year and three digit dayOfYear (yyyyDDD).
		/// </summary>
		public const string BasicOrdinalDate = "basic_ordinal_date";

		/// <summary>
		/// A formatter for a full ordinal date and time, using a four digit year and three digit dayOfYear (yyyyDDD’T'HHmmss.SSSZ).
		/// </summary>
		public const string BasicOrdinalDateTime = "basic_ordinal_date_time";

		/// <summary>
		/// A formatter for a full ordinal date and time without millis, using a four digit year and three digit dayOfYear (yyyyDDD’T'HHmmssZ).
		/// </summary>
		public const string BasicOrdinalDateTimeNoMillis = "basic_ordinal_date_time_no_millis";

		/// <summary>
		/// A basic formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, three digit millis, and time zone offset (HHmmss.SSSZ).
		/// </summary>
		public const string BasicTime = "basic_time";

		/// <summary>
		/// A basic formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, and time zone offset (HHmmssZ).
		/// </summary>
		public const string BasicTimeNoMillis = "basic_time_no_millis";

		/// <summary>
		/// A basic formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, three digit millis, and time zone off set prefixed by T ('T’HHmmss.SSSZ).
		/// </summary>
		public const string BasicTTime = "basic_t_time";

		/// <summary>
		/// A basic formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, and time zone offset prefixed by T ('T’HHmmssZ).
		/// </summary>
		public const string BasicTTimeNoMillis = "basic_t_time_no_millis";

		/// <summary>
		/// A basic formatter for a full date as four digit weekyear, two digit week of weekyear, and one digit day of week (xxxx’W'wwe).
		/// </summary>
		public const string BasicWeekDate = "basic_week_date";

		/// <summary>
		/// A basic formatter that combines a basic weekyear date and time, separated by a T (xxxx’W'wwe’T'HHmmss.SSSZ).
		/// </summary>
		public const string BasicWeekDateTime = "basic_week_date_time";

		/// <summary>
		/// A basic formatter that combines a basic weekyear date and time without millis, separated by a T (xxxx’W'wwe’T'HHmmssZ).
		/// </summary>
		public const string BasicWeekDateTimeNoMillis = "basic_week_date_time_no_millis";

		/// <summary>
		/// A formatter for a full date as four digit year, two digit month of year, and two digit day of month (yyyy-MM-dd).
		/// </summary>
		public const string Date = "date";

		/// <summary>
		/// A formatter that combines a full date and two digit hour of day.
		/// </summary>
		public const string DateHour = "date_hour";

		/// <summary>
		/// A formatter that combines a full date, two digit hour of day, and two digit minute of hour.
		/// </summary>
		public const string DateHourMinute = "date_hour_minute";

		/// <summary>
		/// A formatter that combines a full date, two digit hour of day, two digit minute of hour, and two digit second of minute.
		/// </summary>
		public const string DateHourMinuteSecond = "date_hour_minute_second";

		/// <summary>
		/// A formatter that combines a full date, two digit hour of day, two digit minute of hour, two digit second of minute, and three digit fraction of second (yyyy-MM-dd’T'HH:mm:ss.SSS).
		/// </summary>
		public const string DateHourMinuteSecondFraction = "date_hour_minute_second_fraction";

		/// <summary>
		/// A formatter that combines a full date, two digit hour of day, two digit minute of hour, two digit second of minute, and three digit fraction of second (yyyy-MM-dd’T'HH:mm:ss.SSS).
		/// </summary>
		public const string DateHourMinuteSecondMillis = "date_hour_minute_second_millis";

		/// <summary>
		/// a generic ISO datetime parser where the date is mandatory and the time is optional.
		/// </summary>
		public const string DateOptionalTime = "date_optional_time";

		/// <summary>
		/// A formatter that combines a full date and time, separated by a T (yyyy-MM-dd’T'HH:mm:ss.SSSZZ).
		/// </summary>
		public const string Date_Time = "date_time";

		/// <summary>
		/// A formatter that combines a full date and time without millis, separated by a T (yyyy-MM-dd’T'HH:mm:ssZZ).
		/// </summary>
		public const string DateTimeNoMillis = "date_time_no_millis";

		/// <summary>
		/// A formatter for a two digit hour of day.
		/// </summary>
		public const string Hour = "hour";

		/// <summary>
		/// A formatter for a two digit hour of day and two digit minute of hour.
		/// </summary>
		public const string HourMinute = "hour_minute";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, and two digit second of minute.
		/// </summary>
		public const string HourMinuteSecond = "hour_minute_second";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, and three digit fraction of second (HH:mm:ss.SSS).
		/// </summary>
		public const string HourMinuteSecondFraction = "hour_minute_second_fraction";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, and three digit fraction of second (HH:mm:ss.SSS).
		/// </summary>
		public const string HourMinuteSecondMillis = "hour_minute_second_millis";

		/// <summary>
		/// A formatter for a full ordinal date, using a four digit year and three digit dayOfYear (yyyy-DDD).
		/// </summary>
		public const string OrdinalDate = "ordinal_date";

		/// <summary>
		/// A formatter for a full ordinal date and time, using a four digit year and three digit dayOfYear (yyyy-DDD’T'HH:mm:ss.SSSZZ).
		/// </summary>
		public const string OrdinalDateTime = "ordinal_date_time";

		/// <summary>
		/// A formatter for a full ordinal date and time without millis, using a four digit year and three digit dayOfYear (yyyy-DDD’T'HH:mm:ssZZ).
		/// </summary>
		public const string OrdinalDateTimeNoMillis = "ordinal_date_time_no_millis";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, three digit fraction of second, and time zone offset (HH:mm:ss.SSSZZ).
		/// </summary>
		public const string Time = "time";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, and time zone offset (HH:mm:ssZZ).
		/// </summary>
		public const string TimeNoMillis = "time_no_millis";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, three digit fraction of second, and time zone offset prefixed by T ('T’HH:mm:ss.SSSZZ).
		/// </summary>
		public const string TTime = "t_time";

		/// <summary>
		/// A formatter for a two digit hour of day, two digit minute of hour, two digit second of minute, and time zone offset prefixed by T ('T’HH:mm:ssZZ).
		/// </summary>
		public const string TTimeNoMillis = "t_time_no_millis";

		/// <summary>
		/// A formatter for a full date as four digit weekyear, two digit week of weekyear, and one digit day of week (xxxx-'W’ww-e).
		/// </summary>
		public const string WeekDate = "week_date";

		/// <summary>
		/// A formatter that combines a full weekyear date and time, separated by a T (xxxx-'W’ww-e’T'HH:mm:ss.SSSZZ).
		/// </summary>
		public const string WeekDateTime = "week_date_time";

		/// <summary>
		/// A formatter that combines a full weekyear date and time without millis, separated by a T (xxxx-'W’ww-e’T'HH:mm:ssZZ).
		/// </summary>
		public const string WeekDateTimeNoMillis = "weekDateTimeNoMillis";

		/// <summary>
		/// A formatter for a four digit weekyear.
		/// </summary>
		public const string WeekYear = "week_year";

		/// <summary>
		/// A formatter for a four digit weekyear and two digit week of weekyear.
		/// </summary>
		public const string WeekyearWeek = "weekyearWeek";

		/// <summary>
		/// A formatter for a four digit weekyear, two digit week of weekyear, and one digit day of week.
		/// </summary>
		public const string WeekyearWeekDay = "weekyearWeekDay";

		/// <summary>
		/// A formatter for a four digit year.
		/// </summary>
		public const string Year = "year";

		/// <summary>
		/// A formatter for a four digit year and two digit month of year.
		/// </summary>
		public const string YearMonth = "year_month";

		/// <summary>
		/// A formatter for a four digit year, two digit month of year, and two digit day of month.
		/// </summary>
		public const string YearMonthDay = "year_month_day";

	}
}
