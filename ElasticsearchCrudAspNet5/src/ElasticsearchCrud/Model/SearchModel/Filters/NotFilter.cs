using System.Collections.Generic;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.Model.SearchModel.Filters
{
	public class NotFilter : IFilter
	{
		private IFilter _not;
		private bool _notSet;


		public NotFilter(IFilter not)
		{
			Not = not;
		}

		public IFilter Not
		{
			get { return _not; }
			set
			{
				_not = value;
				_notSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("not");
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			WriteNotFilter(elasticsearchCrudJsonWriter);

			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
		}

		private void WriteNotFilter(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_notSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("filter");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				_not.WriteJson(elasticsearchCrudJsonWriter);
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}