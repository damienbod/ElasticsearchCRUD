using System;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public abstract class SimilarityBase 
	{
		protected bool AnalyzerSet;
		protected string Name;
		protected string Type;

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		protected virtual void WriteJsonBase(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, Action<ElasticsearchCrudJsonWriter> writeFilterSpecific)
		{
			if (AnalyzerSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(Name);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				writeFilterSpecific.Invoke(elasticsearchCrudJsonWriter);

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}
