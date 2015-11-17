using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class StemmerOverrideTokenFilter : AnalysisFilterBase
	{
		private List<string> _rules;
		private bool _rulesSet;
		private string _rulesPath;
		private bool _rulesPathSet;

		/// <summary>
		/// Overrides stemming algorithms, by applying a custom mapping, then protecting these terms from being modified by stemmers. Must be placed before any stemming filters.
        ///
		/// Rules are separated by =>
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public StemmerOverrideTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.StemmerOverride;
		}

		/// <summary>
		/// rules
		/// A list of mapping rules to use.
		/// </summary>
		public List<string> Rules
		{
			get { return _rules; }
			set
			{
				_rules = value;
				_rulesSet = true;
			}
		}

		/// <summary>
		/// rules_path
		/// A path (either relative to config location, or absolute) to a list of mappings.
		/// </summary>
		public string RulesPath
		{
			get { return _rulesPath; }
			set
			{
				_rulesPath = value;
				_rulesPathSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteListValue("rules", _rules, elasticsearchCrudJsonWriter, _rulesSet);
			JsonHelper.WriteValue("rules_path", _rulesPath, elasticsearchCrudJsonWriter, _rulesPathSet);

		}
	}
}