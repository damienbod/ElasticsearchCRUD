namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitYear : TimeUnit
	{
		public TimeUnitYear(int years)
		{
			Years = years;
		}

		public int Years { get; set; }

		public override string GetTimeUnit()
		{
			return Years + "y";
		}
	}
}