namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitNauticalMile : DistanceUnit
	{
		public DistanceUnitNauticalMile(uint nauticalMiles)
		{
			NauticalMiles = nauticalMiles;
		}

		public uint NauticalMiles { get; set; }

		public override string GetDistanceUnit()
		{
			return NauticalMiles + "nmi";
		}
	}
}