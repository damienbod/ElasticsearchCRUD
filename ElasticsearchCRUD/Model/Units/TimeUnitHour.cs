namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitHour : TimeUnit
	{
		public TimeUnitHour(uint hours)
		{
			Hours = hours;
		}

		public uint Hours { get; set; }

		public override string GetTimeUnit()
		{
			return Hours + "h";
		}
	}
}

