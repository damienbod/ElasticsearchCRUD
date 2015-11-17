namespace ElasticsearchCRUD.Model.SearchModel
{
	public class ScriptParameter
	{
		public ScriptParameter(string parameterName, object parameterValue)
		{
			ParameterName = parameterName;
			ParameterValue = parameterValue;
		}

		public string ParameterName { get; set; }
		public object ParameterValue { get; set; }
	}
}