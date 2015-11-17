using System.Collections.Generic;

namespace ElasticsearchCRUD.ContextAlias.AliasModel
{
	// "actions" : [
	//	 { "remove" : { "index" : "test1", "alias" : "alias1" } },
	//	 { "add" : { "index" : "test1", "alias" : "alias2" } }
	// ]
	public class AliasParameters
	{
		private List<AliasBaseParameters> _actions;
		private bool _actionsSet;

		public List<AliasBaseParameters> Actions
		{
			get { return _actions; }
			set
			{
				_actions = value;
				_actionsSet = true;
			}
		}

		public void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			if (_actionsSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName("actions");
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartArray();

				foreach (var item in _actions)
				{
					elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();
					item.WriteJson(elasticsearchCrudJsonWriter);
					elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
				}
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndArray();
				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}

		public override string ToString()
		{
			var elasticsearchCrudJsonWriter = new ElasticsearchCrudJsonWriter();
			WriteJson(elasticsearchCrudJsonWriter);
			return elasticsearchCrudJsonWriter.GetJsonString();
		}
	}
}