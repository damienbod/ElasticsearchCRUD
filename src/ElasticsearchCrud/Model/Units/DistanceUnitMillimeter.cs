namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitMillimeter : DistanceUnit
	{
		public DistanceUnitMillimeter(uint millimeters)
		{
			Millimeters = millimeters;
		}

		public uint Millimeters { get; set; }

		public override string GetDistanceUnit()
		{
			return Millimeters + "mm";
		}
	}
}