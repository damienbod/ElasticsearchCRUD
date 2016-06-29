namespace ElasticsearchCRUD.Model.Units
{
	/// <summary>
	/// Wherever distances need to be specified, such as the distance parameter in the Geo Distance Filter), 
	/// the default unit if none is specified is the meter. Distances can be specified in other units, such as "1km" or "2mi" (2 miles).
	/// </summary>
	public abstract class DistanceUnit
	{
		public abstract string GetDistanceUnit();
	}
}