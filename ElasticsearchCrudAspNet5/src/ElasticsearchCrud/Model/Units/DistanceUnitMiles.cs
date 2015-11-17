namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitMile : DistanceUnit
	{
		public DistanceUnitMile(uint miles)
		{
			Miles = miles;
		}

		public uint Miles { get; set; }

		public override string GetDistanceUnit()
		{
			return Miles + "miles";
		}
	}
}