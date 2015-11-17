namespace ElasticsearchCRUD.Model.Units
{
	public class DistanceUnitKilometer : DistanceUnit
	{
		public DistanceUnitKilometer(uint kilometers)
		{
			Kilometers = kilometers;
		}

		public uint Kilometers { get; set; }

		public override string GetDistanceUnit()
		{
			return Kilometers + "km";
		}
	}
}