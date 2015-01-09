namespace ElasticsearchCRUD.Model.Units
{
	public class TimeUnitDay : TimeUnit
	{
		public TimeUnitDay(int days)
		{
			Days = days;
		}

		public int Days { get; set; }

		public override string GetTimeUnit()
		{
			return Days + "d";
		}
	}
}