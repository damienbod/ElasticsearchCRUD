namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitInch : DistanceUnit
	{
		public DistanceUnitInch(uint inches)
		{
			Inches = inches;
		}

		public uint Inches { get; set; }

		public override string GetDistanceUnit()
		{
			return Inches + "inch";
		}
	}
}

