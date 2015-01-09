namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitMinute : TimeUnit
	{
		public TimeUnitMinute(int minutes)
		{
			Minutes = minutes;
		}

		public int Minutes { get; set; }

		public override string GetTimeUnit()
		{
			return Minutes + "m";
		}
	}
}