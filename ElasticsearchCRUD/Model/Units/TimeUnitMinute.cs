namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitMinute : TimeUnit
	{
		public TimeUnitMinute(uint minutes)
		{
			Minutes = minutes;
		}

		public uint Minutes { get; set; }

		public override string GetTimeUnit()
		{
			return Minutes + "m";
		}
	}
}