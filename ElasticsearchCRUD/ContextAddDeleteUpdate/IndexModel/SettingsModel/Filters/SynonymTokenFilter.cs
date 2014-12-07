using System.Collections.Generic;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Utils;

namespace ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class SynonymTokenFilter : AnalysisFilterBase
	{
		private bool _ignoreCase;
		private bool _ignoreCaseSet;
		private string _synonymsPath;
		private bool _synonymsPathSet;
		private bool _expand;
		private bool _expandSet;
		private List<string> _synonyms;
		private bool _synonymsSet;

		/// <summary>
		/// The synonym token filter allows to easily handle synonyms during the analysis process. Synonyms are configured using a configuration file. 
		/// Additional settings are: ignore_case (defaults to false), and expand (defaults to true).
		///
		/// The tokenizer parameter controls the tokenizers that will be used to tokenize the synonym, and defaults to the whitespace tokenizer.
		/// </summary>
		/// <param name="name">name for the custom filter</param>
		public SynonymTokenFilter(string name)
		{
			AnalyzerSet = true;
			Name = name.ToLower();
			Type = DefaultTokenFilters.Synonym;
		}

		/// <summary>
		/// synonyms_path
		/// </summary>
		public string SynonymsPath
		{
			get { return _synonymsPath; }
			set
			{
				_synonymsPath = value;
				_synonymsPathSet = true;
			}
		}

		public bool IgnoreCase
		{
			get { return _ignoreCase; }
			set
			{
				_ignoreCase = value;
				_ignoreCaseSet = true;
			}
		}

		public bool Expand
		{
			get { return _expand; }
			set
			{
				_expand = value;
				_expandSet = true;
			}
		}

		/// <summary>
		/// Two synonym formats are supported: Solr, WordNet.
		/// These can be defined directly with this parameter.
		/// </summary>
		public List<string> Synonyms
		{
			get { return _synonyms; }
			set
			{
				_synonyms = value;
				_synonymsSet = true;
			}
		}

		public override void WriteJson(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			base.WriteJsonBase(elasticsearchCrudJsonWriter, WriteValues);
		}

		private void WriteValues(ElasticsearchCrudJsonWriter elasticsearchCrudJsonWriter)
		{
			JsonHelper.WriteValue("type", Type, elasticsearchCrudJsonWriter);
			JsonHelper.WriteValue("synonyms_path", _synonymsPath, elasticsearchCrudJsonWriter, _synonymsPathSet);
			JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticsearchCrudJsonWriter, _ignoreCaseSet);
			JsonHelper.WriteValue("expand", _expand, elasticsearchCrudJsonWriter, _expandSet);
			JsonHelper.WriteListValue("synonyms", _synonyms, elasticsearchCrudJsonWriter, _synonymsSet);
			
		}
	}
}