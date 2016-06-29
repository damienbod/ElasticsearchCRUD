namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitMeter : DistanceUnit
	{
		public DistanceUnitMeter(uint meters)
		{
			Meters = meters;
		}

		public uint Meters { get; set; }

		public override string GetDistanceUnit()
		{
			return Meters + "m";
		}
	}
}