namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitCentimeter : DistanceUnit
	{
		public DistanceUnitCentimeter(uint centimeters)
		{
			Centimeters = centimeters;
		}

		public uint Centimeters { get; set; }

		public override string GetDistanceUnit()
		{
			return Centimeters + "cm";
		}
	}
}

