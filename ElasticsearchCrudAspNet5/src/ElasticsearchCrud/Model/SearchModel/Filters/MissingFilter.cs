using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class MissingFilter : IFilter
	{
		private readonly string _field;
		private bool _existence;
		private bool _existenceSet;
		private bool _nullValue;
		private bool _nullValueSet;

		public MissingFilter(string field)
		{
			_field = field;
		}

		/// <summary>
		/// existence
		/// When the existence parameter is set to true (the default), the missing filter will include documents where the field has no values
		/// </summary>
		public bool Existence
		{
			get { return _existence; }
			set
			{
				_existence = value;
				_existenceSet = true;
			}
		}
		
		/// <summary>
		/// null_value
		/// When the null_value parameter is set to true, the missing filter will include documents where the field contains a null value
		/// </summary>
		public bool NullValue
		{
			get { return _nullValue; }
			set
			{
				_nullValue = value;
				_nullValueSet = true;
			}
		}
		
		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("missing");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
			JsonHelper.WriteValue("field", _field, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("existence", _existence, elasticsearchCrudJsonWriter, _existenceSet);
			JsonHelper.WriteValue("null_value", _nullValue, elasticsearchCrudJsonWriter, _nullValueSet);
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();

		}
	}
}
