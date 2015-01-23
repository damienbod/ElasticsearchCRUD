using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Queries
{
	public class IdsQuery: IQuery
	{
		private readonly List<object> _ids;
		private string _type;
		private bool _typeSet;

		/// <summary>
		/// Filters documents that only have the provided ids. Note, this filter does not require the _id field to be indexed since it works using the _uid field.
		/// </summary>
		/// <param name="ids"></param>
		public IdsQuery(List<object> ids)
		{
			_ids = ids;
		}

		public string Type
		{
			get { return _type; }
			set
			{
				_type = value;
				_typeSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("ids");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("type", _type, elasticsearchCrudJsonWriter, _typeSet);
			JsonHelper.WriteListValue("values", _ids, elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}
	}
}
