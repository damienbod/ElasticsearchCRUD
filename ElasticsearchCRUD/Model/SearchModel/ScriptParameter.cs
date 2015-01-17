namespace ElasticsearchCRUD.Model.SearchModel
{
	public class ScriptParameter
	{
		public ScriptParameter(string fieldName, object fieldValue)
		{
			FieldName = fieldName;
			FieldValue = fieldValue;
		}

		public string FieldName { get; set; }
		public object FieldValue { get; set; }
	}
}