namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitHour : TimeUnit
	{
		public TimeUnitHour(int hours)
		{
			Hours = hours;
		}

		public int Hours { get; set; }

		public override string GetTimeUnit()
		{
			return Hours + "h";
		}
	}
}

