namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitWeek : TimeUnit
	{
		public TimeUnitWeek(uint weeks)
		{
			Weeks = weeks;
		}

		public uint Weeks { get; set; }

		public override string GetTimeUnit()
		{
			return Weeks + "w";
		}
	}
}