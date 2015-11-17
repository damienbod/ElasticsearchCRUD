namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitSecond : TimeUnit
	{
		public TimeUnitSecond(uint seconds)
		{
			Seconds = seconds;
		}

		public uint Seconds { get; set; }

		public override string GetTimeUnit()
		{
			return Seconds + "s";
		}
	}
}