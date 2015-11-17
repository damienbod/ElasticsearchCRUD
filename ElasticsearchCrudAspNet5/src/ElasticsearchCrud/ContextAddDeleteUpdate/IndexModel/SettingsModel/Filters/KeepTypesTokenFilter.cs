using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class KeepTypesTokenFilter : AnalysisFilterBase
	{
		private List<string> _types;
		private bool _typesSet;

		/// <summary>
		/// A token filter of type keep_types that only keeps tokens with a token type contained in a predefined set.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public KeepTypesTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.KeepTypes;
		}

		/// <summary>
		/// A list of types to keep 
		/// </summary>
		public List<string> Types
		{
			get { return _types; }
			set
			{
				_types = value;
				_typesSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("types", _types, elasticsearchCrudJsonWriter, _typesSet);
		}
	}
}