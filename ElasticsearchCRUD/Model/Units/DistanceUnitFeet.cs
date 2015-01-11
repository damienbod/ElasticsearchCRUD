namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitFeet : DistanceUnit
	{
		public DistanceUnitFeet(uint feet)
		{
			Feet = feet;
		}

		public uint Feet { get; set; }

		public override string GetDistanceUnit()
		{
			return Feet + "feet";
		}
	}
}