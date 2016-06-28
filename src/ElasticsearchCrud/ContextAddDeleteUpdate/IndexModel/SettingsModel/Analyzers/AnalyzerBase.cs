using System;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public abstract class AnalyzerBase
	{
		protected bool AnalyzerSet;
		protected string Name;
		protected string Type;
		private string _tokenizer;
		private bool _tokenizerSet;

		public abstract void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter);

		public string Tokenizer
		{
			get { return _tokenizer; }
			set
			{
				_tokenizer = value;
				_tokenizerSet = true;
			}
		}

		protected virtual void WriteJsonBase(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter, Action<ElasticsearchCrudJsonWriter> writeFilterSpecific)
		{
			if (AnalyzerSet)
			{
				elasticsearchCrudJsonWriter.JsonWriter.WritePropertyName(Name);
				elasticsearchCrudJsonWriter.JsonWriter.WriteStartObject();

				JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);	
				JsonHelper.WriteValue("tokenizer", _tokenizer, elasticsearchCrudJsonWriter, _tokenizerSet);	
				writeFilterSpecific.Invoke(elasticsearchCrudJsonWriter);

				elasticsearchCrudJsonWriter.JsonWriter.WriteEndObject();
			}
		}
	}
}