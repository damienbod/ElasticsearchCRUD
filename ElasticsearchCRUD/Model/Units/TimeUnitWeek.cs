namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitWeek : TimeUnit
	{
		public TimeUnitWeek(int weeks)
		{
			Weeks = weeks;
		}

		public int Weeks { get; set; }

		public override string GetTimeUnit()
		{
			return Weeks + "w";
		}
	}
}