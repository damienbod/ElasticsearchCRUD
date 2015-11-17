namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitDay : TimeUnit
	{
		public TimeUnitDay(uint days)
		{
			Days = days;
		}

		public uint Days { get; set; }

		public override string GetTimeUnit()
		{
			return Days + "d";
		}
	}
}