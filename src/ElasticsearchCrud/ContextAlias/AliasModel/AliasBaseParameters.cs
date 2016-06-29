using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAlias.AliasModel
{
	public abstract class AliasBaseParameters
	{
		private readonly string _alias;
		private readonly string _index;

		protected AliasBaseParameters(string alias, string index)
		{
			MappingUtils.GuardAgainstBadIndexName(alias);
			MappingUtils.GuardAgainstBadIndexName(index);

			_alias = alias;
			_index = index;
		}

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		protected void WriteInternalJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, AliasAction aliasAction, Action<ElasticsearchCrudJsonWriter> writeFilterSpecific = null)
		{
			elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(aliasAction.ToString());
			elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

			JsonHelper.WriteValue("index", _index, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("alias", _alias, elasticsearchCrudJsonWriter);
			if (writeFilterSpecific != null)
			{
				writeFilterSpecific.Invoke(elasticsearchCrudJsonWriter);
			}
			elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();		
		}
	}
}