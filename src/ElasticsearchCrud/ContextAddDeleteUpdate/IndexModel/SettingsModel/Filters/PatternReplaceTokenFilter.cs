using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class PatternReplaceTokenFilter : AnalysisFilterBase
	{
		private string _pattern;
		private bool _patternSet;
		private string _replacement;
		private bool _replacementSet;

		/// <summary>
		/// The pattern_replace token filter allows to easily handle string replacements based on a regular expression. 
		/// The regular expression is defined using the pattern parameter, and the replacement string can be provided using the replacement parameter
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public PatternReplaceTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.PatternReplace;
		}

		public string Pattern
		{
			get { return _pattern; }
			set
			{
				_pattern = value;
				_patternSet = true;
			}
		}

		public string Replacement
		{
			get { return _replacement; }
			set
			{
				_replacement = value;
				_replacementSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("pattern", _pattern, elasticsearchCrudJsonWriter, _patternSet);
			JsonHelper.WriteValue("replacement", _replacement, elasticsearchCrudJsonWriter, _replacementSet);
		}
	}
}