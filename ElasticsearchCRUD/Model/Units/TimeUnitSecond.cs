namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitSecond : TimeUnit
	{
		public TimeUnitSecond(int seconds)
		{
			Seconds = seconds;
		}

		public int Seconds { get; set; }

		public override string GetTimeUnit()
		{
			return Seconds + "s";
		}
	}
}