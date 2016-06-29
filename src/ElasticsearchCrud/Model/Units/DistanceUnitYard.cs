namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitYard : DistanceUnit
	{
		public DistanceUnitYard(uint yards)
		{
			Yards = yards;
		}

		public uint Yards { get; set; }

		public override string GetDistanceUnit()
		{
			return Yards + "yards";
		}
	}
}