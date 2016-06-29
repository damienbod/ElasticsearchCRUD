namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitYear : TimeUnit
	{
		public TimeUnitYear(uint years)
		{
			Years = years;
		}

		public uint Years { get; set; }

		public override string GetTimeUnit()
		{
			return Years + "y";
		}
	}
}